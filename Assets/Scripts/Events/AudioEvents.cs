using System;
using UnityEngine;

namespace Events
{
    public static class AudioEvents
    {
        public static event Action OnStartRecordingRequest;
        public static void StartRecordingRequest()
        {
            OnStartRecordingRequest?.Invoke();
        }
        
        public static event Action OnStopRecordingRequest;
        public static void StopRecordingRequest()
        {
            OnStopRecordingRequest?.Invoke();
        }
        
        public static event Action OnStartAnalyzingRequest;
        public static void StartAnalyzingRequest()
        {
            OnStartAnalyzingRequest?.Invoke();
        }
        
        public static event Action OnStopAnalyzingRequest;
        public static void StopAnalyzingRequest()
        {
            OnStopAnalyzingRequest?.Invoke();
        }
        
        public static event Action OnStartAnalyzing;
        public static void StartAnalyzing()
        {
            OnStartAnalyzing?.Invoke();
        }
        
        public static event Action OnStopAnalyzing;
        public static void StopAnalyzing()
        {
            OnStopAnalyzing?.Invoke();
        }
        
        public static event Action OnStartedTuning;
        public static void StartedTuning()
        {
            OnStartedTuning?.Invoke();
        }

        public static event Action<string, float> OnTuningProgress;
        public static void TuningProgress(string tuning, float deviation)
        {
            OnTuningProgress?.Invoke(tuning, deviation);
        }

        public static event Action OnStringTuned;
        public static void StringTuned()
        {
            OnStringTuned?.Invoke();
        }
        
        public static event Action OnStartTuningRequest;
        public static void StartTuningRequest()
        {
            OnStartTuningRequest?.Invoke();
        }
        
        public static event Action OnStopTuningRequest;
        public static void StopTuningRequest()
        {
            OnStopTuningRequest?.Invoke();
        }
        
        public static event Action<string> OnNoteDetected;
        public static void NoteDetected(string note)
        {
            OnNoteDetected?.Invoke(note);
        }
    }
}