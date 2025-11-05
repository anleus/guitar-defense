using System;
using FMOD;

namespace Models
{
    [Serializable]
    public class DeviceInfo
    {
        public int Index { get; }
        public string Name { get; }
        public Guid Guid { get; }
        public int SampleRate { get; }
        public SPEAKERMODE SpeakerMode { get; }
        public int Channels { get; }
        public DRIVER_STATE DriverState { get; }

        public DeviceInfo(int index, string name, Guid guid, int sampleRate, SPEAKERMODE speakerMode, int channels,
            DRIVER_STATE driverState)
        {
            Index = index;
            Name = name;
            Guid = guid;
            SampleRate = sampleRate;
            SpeakerMode = speakerMode;
            Channels = channels;
            DriverState = driverState;
        }
    }
}