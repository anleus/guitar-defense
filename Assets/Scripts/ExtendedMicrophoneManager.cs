using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ExtendedMicrophoneManager : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private MicrophoneDropdown microphoneDropdown;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text noteText;
    [SerializeField] private TMP_Text chordText;

    [Header("Parámetros")]
    private int sampleRate = 44100;
    private int sampleWindow = 4096;
    private int sampleSpectrum = 1024;
    private float[] audioSamples;
    private float[] spectrum;
    private AudioClip microphoneClip;

    // Para suavizar frecuencia detectada
    private Queue<float> lastFrequencies = new Queue<float>();
    private int smoothWindow = 5;

    public void StartRecording()
    {
        if (!microphoneDropdown.AvailableMicros())
        {
            Debug.LogWarning("No hay dispositivos disponibles");
            return;
        }

        microphoneClip = Microphone.Start(microphoneDropdown.GetSelectedMicro(), true, 10, sampleRate);
        audioSamples = new float[sampleWindow];
        spectrum = new float[sampleSpectrum];

        Debug.Log("Grabando de: " + microphoneDropdown.GetSelectedMicro());
    }

    private void Update()
    {
        if (microphoneClip == null) return;

        int micPos = Microphone.GetPosition(microphoneDropdown.GetSelectedMicro()) - sampleWindow;
        if (micPos < 0) return;

        microphoneClip.GetData(audioSamples, micPos);

        // Detección de volumen
        float volume = GetVolume(audioSamples);
        if (volume < 0.01f)
        {
            noteText.text = "---";
            chordText.text = "---";
            return;
        }

        // Detección de nota (autocorrelación + suavizado)
        float frequency = DetectPitch(audioSamples, sampleRate);
        if (frequency >= 60f && frequency <= 1300f)
        {
            frequency = SmoothFrequency(frequency);
            string note = FrequencyToNoteName(frequency);
            noteText.text = note;
        }
        else
        {
            noteText.text = "---";
        }

        // Detección de acorde (FFT)
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        string chord = DetectChord(spectrum, sampleRate);
        chordText.text = chord;
    }

    public void StopRecording()
    {
        if (!microphoneDropdown.AvailableMicros()) return;

        string selectedMicro = microphoneDropdown.GetSelectedMicro();
        audioSource.Stop();
        Microphone.End(selectedMicro);

        Debug.Log("Grabación detenida de: " + selectedMicro);
    }

    // ============================================================
    // 🔹 Utilidades de análisis de audio
    // ============================================================

    private float GetVolume(float[] data)
    {
        // RMS (Root Mean Square): mide la "potencia" del sonido
        float sum = 0f;
        foreach (float s in data) sum += s * s;
        return Mathf.Sqrt(sum / data.Length);
    }

    private float DetectPitch(float[] data, float sampleRate)
    {
        int bestLag = 0;
        float bestCorr = 0f;

        for (int lag = 20; lag < data.Length / 2; lag++)
        {
            float sum = 0f;
            for (int i = 0; i < data.Length - lag; i++)
                sum += data[i] * data[i + lag];

            if (sum > bestCorr)
            {
                bestCorr = sum;
                bestLag = lag;
            }
        }

        if (bestLag == 0) return 0f;
        return sampleRate / bestLag;
    }

    private float SmoothFrequency(float newFreq)
    {
        lastFrequencies.Enqueue(newFreq);
        if (lastFrequencies.Count > smoothWindow) lastFrequencies.Dequeue();
        return lastFrequencies.Average();
    }

    private string FrequencyToNoteName(float freq)
    {
        if (freq <= 0f) return "???";

        int midi = Mathf.RoundToInt(12f * Mathf.Log(freq / 440f, 2f) + 69f);
        int octave = (midi / 12) - 1;
        string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        string note = names[midi % 12];
        return $"{note}{octave}";
    }

    // ============================================================
    // 🔹 Detección de acordes
    // ============================================================

    private string DetectChord(float[] spectrum, int sampleRate)
    {
        List<string> detectedNotes = new List<string>();

        // 1. Buscar picos en el espectro
        for (int i = 1; i < spectrum.Length - 1; i++)
        {
            if (spectrum[i] > 0.01f && spectrum[i] > spectrum[i - 1] && spectrum[i] > spectrum[i + 1])
            {
                float freq = i * sampleRate / (2f * spectrum.Length);
                string note = FrequencyToNoteName(freq);

                if (!detectedNotes.Contains(note))
                    detectedNotes.Add(note);
            }
        }

        // 2. Diccionario básico de acordes
        Dictionary<string, string[]> chords = new Dictionary<string, string[]>
        {
            { "C",  new string[]{ "C", "E", "G" } },
            { "Cm", new string[]{ "C", "D#", "G" } },
            { "D",  new string[]{ "D", "F#", "A" } },
            { "Dm", new string[]{ "D", "F", "A" } },
            { "E",  new string[]{ "E", "G#", "B" } },
            { "Em", new string[]{ "E", "G", "B" } },
            { "F",  new string[]{ "F", "A", "C" } },
            { "G",  new string[]{ "G", "B", "D" } },
            { "Am", new string[]{ "A", "C", "E" } },
            { "A", new string[] { "A", "D#", "E"} }
        };

        // 3. Buscar coincidencia de acordes
        foreach (var chord in chords)
        {
            if (chord.Value.All(n => detectedNotes.Any(d => d.StartsWith(n))))
                return chord.Key;
        }

        return "---"; // no se encontró acorde
    }
}
