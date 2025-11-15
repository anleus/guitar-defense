using System.Collections;
using Core.Audio.Processors;
using Events;
using Models;
using Models.Enums;
using UnityEngine;
using UnityEngine.Audio;

namespace Core.Audio
{
    [RequireComponent(typeof(FMODMicroRecorder))]
    public class AudioManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private FMODMicroRecorder fmodMicroRecorder;
        [SerializeField] private Tuner tuner;
        private PitchProcessor pitchProcessor;
        
        [Header("Status")]
        [SerializeField] private AudioManagerState currentState = AudioManagerState.Idle;
        
        private Coroutine analyseSpectrumCoroutine;
        [SerializeField] private float refreshInterval = 0.05f;
        [SerializeField] private int sampleCount = 4096;
        
        private void OnEnable()
        {
            AudioEvents.OnStartRecordingRequest += StartRecording;
            AudioEvents.OnStopRecordingRequest += StopRecording;
            AudioEvents.OnStartAnalyzingRequest += StartAnalyzing;
            AudioEvents.OnStopAnalyzingRequest += StopAnalyzing;
            
            UIEvents.OnMicroSelected += SetCurrentSampleRate;
        }

        private void OnDisable()
        {
            AudioEvents.OnStartRecordingRequest -= StartRecording;
            AudioEvents.OnStopRecordingRequest -= StopRecording;
            AudioEvents.OnStartAnalyzingRequest -= StartAnalyzing;
            AudioEvents.OnStopAnalyzingRequest -= StopAnalyzing;

            UIEvents.OnMicroSelected -= SetCurrentSampleRate;

            StopRecording();
        }

        private void Start()
        {
            pitchProcessor = new PitchProcessor();
        }

        private void StartRecording()
        {
            Debug.Log("StartRecording");
            if (currentState != AudioManagerState.Idle) StopRecording();
                
            fmodMicroRecorder.StartRecording();
            currentState = AudioManagerState.Recording;
        }

        private void StopRecording()
        {
            if (currentState == AudioManagerState.Idle) return;
            
            fmodMicroRecorder.StopRecording();
            currentState = AudioManagerState.Idle;
        }
        
        private void StartAnalyzing()
        {
            if (currentState != AudioManagerState.Recording) return;

            currentState = AudioManagerState.Analyzing;
            analyseSpectrumCoroutine = StartCoroutine(AnalyseSpectrum());
            AudioEvents.StartAnalyzing();
        }

        private void StopAnalyzing()
        {
            if (currentState != AudioManagerState.Analyzing) return;

            if (analyseSpectrumCoroutine == null) return;

            StopCoroutine(analyseSpectrumCoroutine);

            analyseSpectrumCoroutine = null;
            currentState = AudioManagerState.Recording;
            
            AudioEvents.StopAnalyzing();
        }

        private IEnumerator AnalyseSpectrum()
        {
            while (currentState == AudioManagerState.Analyzing)
            {
                var pcmData = fmodMicroRecorder.GetPCMData(sampleCount);
                var noteInfo = pitchProcessor.Process(pcmData);

                if (noteInfo != null)
                {
                    Debug.Log(noteInfo);
                    tuner.UpdateCurrentPitch(noteInfo);
                    AudioEvents.NoteDetected(noteInfo.Name);
                }

                yield return new WaitForSeconds(refreshInterval);
            }
        }

        private void SetCurrentSampleRate(DeviceInfo device)
        {
            pitchProcessor.SetSampleRate(device?.SampleRate ?? 0f);
        }
    }
}