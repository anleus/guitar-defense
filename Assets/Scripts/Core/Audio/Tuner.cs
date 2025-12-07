using System.Collections;
using Events;
using FMODUnity;
using Models;
using UnityEngine;
using Utils;

namespace Core.Audio
{
    public class Tuner : MonoBehaviour
    {
        [SerializeField] private float toleranceCents = 5f;
        [SerializeField] private float checkInterval = 0.1f;
        
        [Header("FMOD events")] 
        [SerializeField] private EventReference stringCheckEvent;

        private bool analyzing;
        private bool uiVisible;
        private bool CanTune => analyzing && uiVisible;
        
        private Coroutine tuningCoroutine;
        private NoteInfo currentNote;
        
        private void OnEnable()
        {
            AudioEvents.OnStartAnalyzing += OnStartAnalyzing;
            AudioEvents.OnStopAnalyzing += OnStopAnalyzing;
            
            UIEvents.OnToggleTuningRequest += OnTuningUIRequest;
            UIEvents.OnVisibleTuningUI += OnVisibleTuningUi;
        }

        private void OnDisable()
        {
            AudioEvents.OnStartAnalyzing -= OnStartAnalyzing;
            AudioEvents.OnStopAnalyzing -= OnStopAnalyzing;
            
            UIEvents.OnToggleTuningRequest -= OnTuningUIRequest;
            UIEvents.OnVisibleTuningUI -= OnVisibleTuningUi;
        }
        
        private void OnStartAnalyzing()
        {
            analyzing = true;
            UpdateTuningState();
        }

        private void OnStopAnalyzing()
        {
            analyzing = false;
            UpdateTuningState();
        }

        private void OnTuningUIRequest()
        {
            if (!analyzing) return;
            UIEvents.ToggleTuningUI();
        }

        private void OnVisibleTuningUi(bool visible)
        {
            uiVisible = visible;
            UpdateTuningState();
        }

        private void UpdateTuningState()
        {
            if (!CanTune) return;
            
            AudioEvents.StartedTuning();
            CoroutineUtils.RestartCoroutine(this, ref tuningCoroutine, TuningProcess(), CanTune);
        }
        
        public void UpdateCurrentPitch(NoteInfo noteInfo)
        {
            if (!CanTune) return;
            currentNote =  noteInfo;
        }

        private IEnumerator TuningProcess()
        {
            foreach (var noteInfo in MusicLibrary.StandardTuning)
            {
                var tuned = false;
                while (!tuned)
                {
                    if (currentNote is null)
                    {
                        yield return new WaitForSeconds(checkInterval);
                        continue;
                    }
                    
                    var deviation = FrequencyToCents(currentNote.Frequency, noteInfo.Value);
                    AudioEvents.TuningProgress(currentNote.Name, deviation);
                    
                    if (Mathf.Abs(deviation) <= toleranceCents)
                    {
                        tuned = true;
                        RuntimeManager.PlayOneShot(stringCheckEvent, transform.position);
                        AudioEvents.StringTuned();
                    }
                    yield return new WaitForSeconds(checkInterval);
                    currentNote = null;
                }
            }
        }
        
        // Converts freq difference to cents (1 semitone = 100 cents)
        private float FrequencyToCents(float freq, float target)
        {
            return 1200f * Mathf.Log(freq / target, 2f);
        }
    }
}