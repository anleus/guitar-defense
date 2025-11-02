using System;
using System.Collections;
using System.Runtime.InteropServices;
using Audio.Models;
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

        public void StartRecording(DeviceInfo deviceInfo)
        {
            exInfo.cbsize = Marshal.SizeOf(typeof(FMOD.CREATESOUNDEXINFO));
            exInfo.numchannels = deviceInfo.Channels;
            exInfo.format = FMOD.SOUND_FORMAT.PCM16;
            exInfo.defaultfrequency = deviceInfo.SampleRate;
            exInfo.length = (uint)deviceInfo.SampleRate * sizeof(short) * (uint)deviceInfo.Channels;
            
            RuntimeManager.CoreSystem.createSound(exInfo.userdata, FMOD.MODE.LOOP_NORMAL | FMOD.MODE.OPENUSER, ref exInfo, out sound);

            RuntimeManager.CoreSystem.recordStart(deviceInfo.Index, sound, true);

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

        public void StopRecording(DeviceInfo deviceInfo)
        {
            if (fftDsp.hasHandle())
            {
                channel.removeDSP(fftDsp);
            }
        
            if (channel.hasHandle())
            {
                channel.stop();
            }
        
            RuntimeManager.CoreSystem.recordStop(deviceInfo.Index);
            sound.release();
        }
        
        public (float[] spectrum, int numBins) GetSpectrumData()
        {
            if (!fftDsp.hasHandle()) return (Array.Empty<float>(), 0);

            fftDsp.getParameterData((int)FMOD.DSP_FFT.SPECTRUMDATA, out var unmanagedData, out uint _);
            var fftData = (FMOD.DSP_PARAMETER_FFT)Marshal.PtrToStructure(unmanagedData, typeof(FMOD.DSP_PARAMETER_FFT));

            if (fftData.numchannels <= 0) return (Array.Empty<float>(), 0);

            var spectrum = new float[fftData.length];
            for (var i = 0; i < fftData.numchannels; i++)
            {
                spectrum = new float[fftData.length];
            }
            fftData.getSpectrum(0, ref spectrum);
            return (spectrum, fftData.length);
        }
    }
}