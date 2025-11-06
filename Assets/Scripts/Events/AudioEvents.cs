using System;
using UnityEngine;

namespace Events
{
    public static class AudioEvents
    {
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
        
        public static event Action<string> OnNoteDetected;
        public static void NoteDetected(string note)
        {
            OnNoteDetected?.Invoke(note);
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
    }
}