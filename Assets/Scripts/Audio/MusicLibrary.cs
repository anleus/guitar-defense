using System;
using System.Collections.Generic;

namespace Audio
{
    public static class MusicLibrary
    {
        private static readonly Dictionary<string, float> GuitarTuning = new()
        {
            { "E2", 82.41f },
            { "A2", 110.00f },
            { "D3", 146.83f },
            { "G3", 196.00f },
            { "B3", 246.94f },
            { "E4", 329.63f }
        };
        
        public static readonly string[] AMajorScaleNotes =
        {
            "A", "B", "C#", "D", "E", "F#", "G#"
        };
    }
}
