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
        
        [Header("Contraints")]
        [SerializeField] private bool isRecording;
        [SerializeField] private bool isAnalyzing;
        private float[] currentSpectrum;
        private int currentNumBins;
        private DeviceInfo currentDevice;
        
        private Coroutine refreshSpectrumCoroutine;
        [SerializeField] private float refreshInterval = 0.05f;

        private void Start()
        {
            pitchProcessor = new PitchProcessor();
        }

        private void Update()
        {
            if (!isAnalyzing || currentSpectrum == null) return;

            var noteInfo = pitchProcessor.Analyze(currentSpectrum, currentSpectrum.Length);
            Debug.Log(noteInfo);
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
            if (refreshSpectrumCoroutine != null)
            {
                StopCoroutine(refreshSpectrumCoroutine);
                refreshSpectrumCoroutine = null;
            }
            if (isAnalyzing) refreshSpectrumCoroutine = StartCoroutine(RefreshSpectrum());
        }

        private IEnumerator RefreshSpectrum()
        {
            while (isRecording && isAnalyzing)
            {
                var spectrum = fmodMicroRecorder.GetSpectrumData();
                currentSpectrum = spectrum;

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
            AddEventListeners();
        }

        private void OnDisable()
        {
            RemoveEventListeners();
            fmodMicroRecorder.StopRecording(currentDevice);
        }

        private void AddEventListeners()
        {
            GameEvents.OnGameLoopStart += () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause += () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop += () => EnableAnalysis(false);
            
            UIEvents.OnMicroSelected += SetCurrentDevice;
        }

        private void RemoveEventListeners()
        {
            GameEvents.OnGameLoopStart -= () => EnableAnalysis(true);
            GameEvents.OnGameLoopPause -= () => EnableAnalysis(false);
            GameEvents.OnGameLoopStop -= () => EnableAnalysis(false);

            UIEvents.OnMicroSelected -= SetCurrentDevice;
        }
    }
}