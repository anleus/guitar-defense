using System;
using Audio.Models;
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

        public static event Action<bool> OnToggleTuner;
        public static void ToggleTuner(bool visible)
        {
            OnToggleTuner?.Invoke(visible);
        }
        
    }
}