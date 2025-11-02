using Audio.Models;
using UnityEngine;

namespace Audio.Processors
{
    public class PitchProcessor
    {
        private float sampleRate = 0f;

        public void SetSampleRate(float sampleRate)
        {
            this.sampleRate = sampleRate;
        }

        public NoteInfo Analyze(float[] fftSpectrum, int numBins)
        {
            if (sampleRate == 0f) return null;
            
            var maxMag = 0f;
            var maxBin = 0;
            //search for the highest frequency band (dominant)
            for (var i = 0; i < fftSpectrum.Length; i++)
            {
                if (fftSpectrum[i] > maxMag)
                {
                    maxMag = fftSpectrum[i];
                    maxBin = i;
                }
            }

            if (!(maxMag > 0f)) return null;
            
            var frequency = maxBin * sampleRate / (float)numBins;
            var note = FrequencyToNote(frequency);
            
            return new NoteInfo(note, frequency);
        }
        
        private string FrequencyToNote(float frequency)
        {
            if (frequency <= 0f) return "---";

            var midi = Mathf.RoundToInt(12f * Mathf.Log(frequency / 440f, 2f) + 69f);
            var octave = (midi / 12) - 1;
            var note = MusicLibrary.Notes[midi % 12];
            return $"{note}{octave}";
        }
    }
}