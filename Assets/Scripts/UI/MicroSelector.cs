using System.Collections;
using System.Collections.Generic;
using Events;
using FMOD;
using FMODUnity;
using Models;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UI
{
    public class MicroSelector : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown deviceDropdown;

        private readonly List<DeviceInfo> deviceOptions = new();

        private int selectedMicroIndex = -1;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            SetMicOptions();
        }

        private void SetMicOptions()
        {
            RuntimeManager.CoreSystem.getRecordNumDrivers(out var numOfDrivers, out var numOfDriversConnected);
            deviceOptions.Clear();
            deviceDropdown.options.Clear();

            if (numOfDriversConnected == 0)
            {
                Debug.Log("Connect a device");
                return;
            }

            var deviceNames = new List<string>();

            for (var i = 0; i < numOfDriversConnected; i++)
            {
                RuntimeManager.CoreSystem.getRecordDriverInfo(
                    i,
                    out var deviceName,
                    100,
                    out var guid,
                    out var systemRate,
                    out var speakerMode,
                    out var channels,
                    out var state
                );
                var connected = state.HasFlag(DRIVER_STATE.CONNECTED);

                //filters for devices that are a microphone
                if (connected && !deviceName.ToLower().Contains("loopback"))
                {
                    var device = new DeviceInfo(i, deviceName, guid, systemRate, speakerMode, channels, state);
                    deviceOptions.Add(device);
                    deviceNames.Add(device.Name);
                }
            }

            deviceDropdown.AddOptions(deviceNames);

            if (deviceOptions.Count <= 0) return;

            selectedMicroIndex = 0;
            UIEvents.MicroSelected(deviceOptions[selectedMicroIndex]);
            Debug.Log("Default selected index " + selectedMicroIndex + " -> " +
                      deviceDropdown.options[selectedMicroIndex].text);
        }


        public void OnOptionSelected(int index)
        {
            selectedMicroIndex = index;
            UIEvents.MicroSelected(deviceOptions[selectedMicroIndex]);
            Debug.Log("Selected index " + index + " -> " + deviceDropdown.options[index].text);
        }
    }
}