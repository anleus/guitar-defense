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
    }
}