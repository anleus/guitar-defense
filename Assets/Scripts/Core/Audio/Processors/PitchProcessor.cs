using System.Linq;
using Models;
using UnityEngine;
using Utils;

namespace Core.Audio.Processors
{
    public class PitchProcessor
    {
        private float sampleRate = 0f;

        public void SetSampleRate(float sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        public NoteInfo Process(float[] data)
        {
            var volume = GetVolume(data);
            if (volume < 0.01f) return null;
            
            var frequency = GetFrequency(data);
            if (frequency is < 60f or > 1300f) return null;

            var note = FrequencyToNote(frequency);
            
            return new NoteInfo(note, frequency);
        }
        
        private static float GetVolume(float[] data)
        {
            var sum = data.Sum(s => s * s);
            return Mathf.Sqrt(sum / data.Length);
        }

        private float GetFrequency(float[] data)
        {
            if (data == null || data.Length == 0 || sampleRate <= 0f) return 0f;

            var bestLag = 0;
            var bestCorr = 0f;
            
            for (var lag = 20; lag < data.Length / 2; lag++)
            {
                var sum = 0f;
                for (var i = 0; i < data.Length - lag; i++)
                    sum += data[i] * data[i + lag];

                if (sum > bestCorr)
                {
                    bestCorr = sum;
                    bestLag = lag;
                }
            }

            if (bestLag == 0) return 0f;

            return sampleRate / (float)bestLag;
        }

        private static string FrequencyToNote(float frequency)
        {
            if (frequency <= 0f) return "---";

            int midi = Mathf.RoundToInt(12f * Mathf.Log(frequency / 440f, 2f) + 69f);
            int octave = (midi / 12) - 1;
            string note = MusicLibrary.Notes[midi % 12];
            return $"{note}{octave}";
        }
    }
}