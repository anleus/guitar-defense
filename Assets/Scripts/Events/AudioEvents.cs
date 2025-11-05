using System;
using UnityEngine;

namespace Events
{
    public static class AudioEvents
    {
        public static event Action<string, float> OnNoteDetected;
        public static void NoteDetected(string note, float frequency)
        {
            OnNoteDetected?.Invoke(note, frequency);
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