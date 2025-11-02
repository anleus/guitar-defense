using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using Utils;

namespace Audio
{
    public class Tuner : MonoBehaviour
    {
        [SerializeField] private float toleranceCents = 5f;
        [SerializeField] private float checkInterval = 0.1f;
        
        private bool canTune;
        private Coroutine tuningCoroutine;
        private string currentNote;
        private float currentFreq;

        private void ManageTuningRoutine(bool uiVisible)
        {
            CoroutineUtils.RestartCoroutine(this, ref tuningCoroutine, TuningProcess(), uiVisible);
        }

        private IEnumerator TuningProcess()
        {
            foreach (var noteInfo in MusicLibrary.StandardTuning)
            {
                var tuned = false;
                while (!tuned)
                {
                    if (currentNote is null or "-")
                    {
                        yield return new WaitForSeconds(checkInterval);
                        continue;
                    }
                    
                    var deviation = FrequencyToCents(currentFreq, noteInfo.Value);
                    AudioEvents.TuningProgress(currentNote, deviation);
                    
                    if (Mathf.Abs(deviation) <= toleranceCents)
                    {
                        tuned = true;
                        AudioEvents.StringTuned();
                    }
                    yield return new WaitForSeconds(checkInterval);
                    currentNote = null;
                }
            }
        }
        
        // Coverts freq difference to cents (1 semitone = 100 cents)
        private float FrequencyToCents(float freq, float target)
        {
            return 1200f * Mathf.Log(freq / target, 2f);
        }

        private void NoteDetected(string note, float frequency)
        {
            if (!canTune) return;
            currentNote =  note;
            currentFreq = frequency;
        }

        private void OnEnable()
        {
            GameEvents.OnGameLoopStart += () => canTune = true;
            
            AudioEvents.OnNoteDetected += NoteDetected;

            UIEvents.OnToggleTuner += ManageTuningRoutine;
        }

        private void OnDisable()
        {
            GameEvents.OnGameLoopStart -= () => canTune = false;
            
            AudioEvents.OnNoteDetected -= NoteDetected;
            
            UIEvents.OnToggleTuner -= ManageTuningRoutine;
        }
    }
}