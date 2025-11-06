using System;
using UnityEngine;

namespace Events
{
    public static class GameEvents
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
    }
}
