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
    }
}