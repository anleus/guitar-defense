using System.Linq;
using UnityEngine;

namespace Processors
{
    public class PitchProcessor
    {
        private readonly int sampleRate;

        public PitchProcessor(int sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        public string Process(float[] samples)
        {
            var volume = GetVolume(samples);
            if (volume < 0.01f) return "---";

            var frequency = GetFrequency(samples);
            if (frequency is < 60f or > 1300f) return "---";
        
            Debug.Log(frequency);

            return FrequencyToNoteName(frequency);
        }

        private float GetVolume(float[] data)
        {
            var sum = data.Sum(s => s * s);
            return Mathf.Sqrt(sum / data.Length);
        }

        private float GetFrequency(float[] data)
        {
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

        private string FrequencyToNoteName(float freq)
        {
            if (freq <= 0f) return "???";

            var midi = Mathf.RoundToInt(12f * Mathf.Log(freq / 440f, 2f) + 69f);
            var octave = (midi / 12) - 1;
            string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
            var note = names[midi % 12];
            return $"{note}{octave}";
        }
    }
}