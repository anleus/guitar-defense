using System.Collections.Generic;

namespace Utils
{
    public static class MusicLibrary
    {
        public static readonly List<string> Notes =  new() {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

        public static readonly Dictionary<string, float> StandardTuning = new()
        {
            { "E2", 82.41f },
            { "A2", 110.00f },
            { "D3", 146.83f },
            { "G3", 196.00f },
            { "B3", 246.94f },
            { "E4", 329.63f }
        };
    }
}