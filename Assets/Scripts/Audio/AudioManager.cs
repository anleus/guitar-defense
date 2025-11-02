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
        
        private DeviceInfo currentDevice;
        
        private Coroutine analyseSpectrumCoroutine;
        [SerializeField] private float refreshInterval = 0.05f;

        private void Start()
        {
            pitchProcessor = new PitchProcessor();
        }

        public void StartRecording()
        {
            fmodMicroRecorder.StartRecording(currentDevice);
            isRecording = true;
        }

        public void StopRecording()
        {
            fmodMicroRecorder.StopRecording(currentDevice);
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
                var (spectrum, numBeans) = fmodMicroRecorder.GetSpectrumData();
                
                var noteInfo = pitchProcessor.Analyze(spectrum, numBeans);
                Debug.Log(noteInfo);
                
                AudioEvents.NoteDetected(noteInfo.Note, noteInfo.Frequency);

                yield return new WaitForSeconds(refreshInterval);
            }
        }

        private void SetCurrentDevice(DeviceInfo device)
        {
            currentDevice = device;
            pitchProcessor.SetSampleRate(device.SampleRate);
        }
        
        private void OnEnable()
        {
            GameEvents.OnGameLoopStart += () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause += () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop += () => EnableAnalysis(false);
            
            UIEvents.OnMicroSelected += SetCurrentDevice;
        }

        private void OnDisable()
        {
            GameEvents.OnGameLoopStart -= () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause -= () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop -= () => EnableAnalysis(false);

            UIEvents.OnMicroSelected -= SetCurrentDevice;
            
            fmodMicroRecorder.StopRecording(currentDevice);
        }
    }
}