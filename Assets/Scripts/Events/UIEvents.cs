using System;
using Models;
using UnityEngine;

namespace Events
{
    public static class UIEvents
    {
        public static event Action<DeviceInfo> OnMicroSelected;
        public static void MicroSelected(DeviceInfo deviceInfo)
        {
            OnMicroSelected?.Invoke(deviceInfo);
        }

        public static event Action OnToggleTuningRequest;
        public static void ToggleTuningRequest()
        {
            OnToggleTuningRequest?.Invoke();
        }

        public static event Action OnToggleTuningUI;

        public static void ToggleTuningUI()
        {
            OnToggleTuningUI?.Invoke();
        }
        
        public static event Action<bool> OnVisibleTuningUI;
        public static void VisibleTuningUI(bool visible)
        {
            OnVisibleTuningUI?.Invoke(visible);
        }

        public static event Action<int> OnRefreshScore;
        public static void RefreshScore(int score)
        {
            OnRefreshScore?.Invoke(score);
        }
        
        public static event Action<string, Sprite> OnRefreshPatternData;
        public static void RefreshPatternData(string patternName, Sprite patternImage)
        {
            OnRefreshPatternData?.Invoke(patternName, patternImage);
        }

        public static event Action<int> OnRefreshLivesIndicator;
        public static void RefreshLivesIndicator(int livesIndicator)
        {
            OnRefreshLivesIndicator?.Invoke(livesIndicator);
        }
    }
}