using System;
using System.Collections;
using System.Runtime.InteropServices;
using Audio.Models;
using Events;
using FMODUnity;
using UnityEngine;

namespace Audio
{
    public class FMODMicroRecorder : MonoBehaviour
    {
        [SerializeField] private float latency = 1f;
    
        private FMOD.Sound sound;
        private FMOD.CREATESOUNDEXINFO exInfo;
        private FMOD.Channel channel;
        private FMOD.ChannelGroup channelGroup;
        private FMOD.DSP fftDsp;
        
        private const int FFTSize = 4096;

        private DeviceInfo currentDevice;

        public void StartRecording()
        {
            if (currentDevice == null) return;
            
            exInfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
            exInfo.numchannels = currentDevice.Channels;
            exInfo.format = FMOD.SOUND_FORMAT.PCM16;
            exInfo.defaultfrequency = currentDevice.SampleRate;
            exInfo.length = (uint)currentDevice.SampleRate * sizeof(short) * (uint)currentDevice.Channels;
            
            RuntimeManager.CoreSystem.createSound(exInfo.userdata, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref exInfo, out sound);

            RuntimeManager.CoreSystem.recordStart(currentDevice.Index, sound, true);

            StartCoroutine(WaitAndPlay());
        }

        //preventive time to avoid playing an empty buffer
        private IEnumerator WaitAndPlay()
        {
            yield return new WaitForSeconds(latency);
            RuntimeManager.CoreSystem.playSound(sound, channelGroup, false, out channel);

            if (RuntimeManager.CoreSystem.createDSPByType(FMOD.DSP_TYPE.FFT, out fftDsp) == FMOD.RESULT.OK)
            {
                fftDsp.setParameterInt((int)FMOD.DSP_FFT.WINDOW, (int)FMOD.DSP_FFT_WINDOW_TYPE.HANNING);
                fftDsp.setParameterInt((int)FMOD.DSP_FFT.WINDOWSIZE, FFTSize);

                channel.addDSP(FMOD.CHANNELCONTROL_DSP_INDEX.HEAD, fftDsp);
            } 
            
            Debug.Log("Ready To Play");
        }

        public void StopRecording()
        {
            if (fftDsp.hasHandle())
            {
                channel.removeDSP(fftDsp);
            }
        
            if (channel.hasHandle())
            {
                channel.stop();
            }
        
            RuntimeManager.CoreSystem.recordStop(currentDevice.Index);
            sound.release();
            currentDevice = null;
        }
        
        public float[] GetPCMData(int sampleCount)
        {
            if (!sound.hasHandle()) return Array.Empty<float>();
            
            RuntimeManager.CoreSystem.getRecordPosition(currentDevice.Index, out uint recordPos);
            
            int bytesPerSample = sizeof(short) * currentDevice.Channels;
            uint bytesToRead = (uint)(sampleCount * bytesPerSample);

            sound.getLength(out uint soundLengthBytes, FMOD.TIMEUNIT.PCMBYTES);

            uint readPos = (uint)(recordPos * (uint)bytesPerSample);
            if (readPos >= bytesToRead)
                readPos -= bytesToRead;
            else
                readPos = soundLengthBytes - (bytesToRead - readPos);

            FMOD.RESULT result = sound.@lock(readPos, bytesToRead, 
                out IntPtr ptr1, out IntPtr ptr2, 
                out uint len1, out uint len2);

            if (result != FMOD.RESULT.OK)
            {
                Debug.LogWarning($"[FMODMicroRecorder] Lock failed: {result}");
                return Array.Empty<float>();
            }

            // 5. Copiar datos de los punteros a un array manejado
            int totalSamples = (int)((len1 + len2) / bytesPerSample);
            float[] pcmData = new float[totalSamples];

            int offset = 0;
            if (len1 > 0)
            {
                short[] temp1 = new short[len1 / sizeof(short)];
                Marshal.Copy(ptr1, temp1, 0, temp1.Length);
                for (int i = 0; i < temp1.Length; i++)
                    pcmData[offset++] = temp1[i] / 32768f; // Normalizar a [-1, 1]
            }

            if (len2 > 0)
            {
                short[] temp2 = new short[len2 / sizeof(short)];
                Marshal.Copy(ptr2, temp2, 0, temp2.Length);
                for (int i = 0; i < temp2.Length; i++)
                    pcmData[offset++] = temp2[i] / 32768f;
            }

            // 6. Liberar el lock
            sound.unlock(ptr1, ptr2, len1, len2);

            return pcmData;
        }
        
        private void OnEnable()
        {
            UIEvents.OnMicroSelected += deviceInfo => currentDevice = deviceInfo;
        }

        private void OnDisable()
        {
            StopRecording();
            UIEvents.OnMicroSelected -= _ => currentDevice = null;
        }
    }
}