using System;
using System.Collections;
using Audio.Models;
using Audio.Processors;
using Events;
using TMPro;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(FMODMicroRecorder))]
    public class AudioManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private FMODMicroRecorder fmodMicroRecorder;
        private PitchProcessor pitchProcessor;
        
        [Header("Constraints")]
        [SerializeField] private bool isRecording;
        [SerializeField] private bool isAnalyzing;
        
        private Coroutine analyseSpectrumCoroutine;
        [SerializeField] private float refreshInterval = 0.05f;
        [SerializeField] private int sampleCount = 4096;

        private void Start()
        {
            pitchProcessor = new PitchProcessor();
        }

        public void StartRecording()
        {
            fmodMicroRecorder.StartRecording();
            isRecording = true;
        }

        public void StopRecording()
        {
            fmodMicroRecorder.StopRecording();
            isRecording = false;
            isAnalyzing = false;
        }
        
        private void EnableAnalysis(bool enable)
        {
            if (!isRecording) return;
            
            isAnalyzing = enable;
            if (analyseSpectrumCoroutine != null)
            {
                StopCoroutine(analyseSpectrumCoroutine);
                analyseSpectrumCoroutine = null;
            }
            if (isAnalyzing) analyseSpectrumCoroutine = StartCoroutine(AnalyseSpectrum());
        }

        private IEnumerator AnalyseSpectrum()
        {
            while (isRecording && isAnalyzing)
            {
                var pcmData = fmodMicroRecorder.GetPCMData(sampleCount);
                
                var noteInfo = pitchProcessor.Process(pcmData);

                if (noteInfo != null)
                {
                    Debug.Log(noteInfo);
                    AudioEvents.NoteDetected(noteInfo.Note, noteInfo.Frequency);
                }

                yield return new WaitForSeconds(refreshInterval);
            }
        }

        private void SetCurrentSampleRate(DeviceInfo device)
        {
            pitchProcessor.SetSampleRate(device?.SampleRate ?? 0f);
        }
        
        private void OnEnable()
        {
            GameEvents.OnGameLoopStart += () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause += () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop += () => EnableAnalysis(false);
            
            UIEvents.OnMicroSelected += SetCurrentSampleRate;
        }

        private void OnDisable()
        {
            GameEvents.OnGameLoopStart -= () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause -= () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop -= () => EnableAnalysis(false);

            UIEvents.OnMicroSelected -= SetCurrentSampleRate;
        }
    }
}