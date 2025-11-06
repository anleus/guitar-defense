using System.Collections;
using Events;
using Models;
using UnityEngine;
using Utils;

namespace Core
{
    public class TestDeviceManager : MonoBehaviour
    {
        private Coroutine startTestCoroutine;
        private Coroutine stopTestCoroutine;

        private void OnEnable()
        {
            UIEvents.OnMicroSelected += SaveDefaultMicro;
        }

        private void OnDisable()
        {
            UIEvents.OnMicroSelected -= SaveDefaultMicro;
        }
        
        private void SaveDefaultMicro(DeviceInfo deviceInfo)
        {
            PlayerPrefs.SetString("deviceName", deviceInfo.Name);
        }
        
        public void TestDevice()
        {
            CoroutineUtils.RestartCoroutine(this, ref startTestCoroutine, StartTest());
        }
        
        public void Next()
        {
            CoroutineUtils.RestartCoroutine(this, ref stopTestCoroutine, StopTest());
        }

        private IEnumerator StartTest()
        {
            GameEvents.StartRecordingRequest();
            yield return new WaitForSeconds(0.1f);
            GameEvents.StartAnalyzingRequest();
            yield return new WaitForSeconds(0.1f);
            UIEvents.ToggleTuningRequest();
        }

        private IEnumerator StopTest()
        {
            UIEvents.ToggleTuningRequest();
            yield return new WaitForSeconds(0.1f);
            GameEvents.StopAnalyzingRequest();
            yield return new WaitForSeconds(0.1f);
            GameEvents.StopRecordingRequest();
        }
    }
}