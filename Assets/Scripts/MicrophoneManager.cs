using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicrophoneManager : MonoBehaviour
{
    [SerializeField] private MicrophoneDropdown microphoneDropdown;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private TMP_Text noteText;

    private int sampleRate = 44100;
    private int sampleWindow = 4096;
    private float[] audioSamples;
    private AudioClip microphoneClip;

    private string lastValidNote = "---";

    public void StartRecording()
    {
        if (!microphoneDropdown.AvailableMicros())
        {
            Debug.LogWarning("No avaliable devices");
        }
        else
        {
            microphoneClip = Microphone.Start(microphoneDropdown.GetSelectedMicro(), true, 10, sampleRate);
            audioSamples = new float[sampleWindow];
        }
    }

    private void Update()
    {
        int micPos = Microphone.GetPosition(microphoneDropdown.GetSelectedMicro()) - sampleWindow;
        if (micPos < 0) return;

        microphoneClip.GetData(audioSamples, micPos);

        float volume = GetVolume(audioSamples);
        if (volume < 0.01f)  // Puedes ajustar el umbral
        {
            noteText.text = "---";
            Debug.Log("Silencio o señal muy débil");
            return;
        }

        float frequency = DetectPitch(audioSamples, sampleRate);
        if (frequency < 60f || frequency > 1300f)
        {
            noteText.text = "---";
            Debug.Log($"Frecuencia fuera de rango: {frequency:F2} Hz");
            return;
        }

        string note = FrequencyToNoteName(frequency);

        noteText.text = note;

        Debug.Log($"Frecuencia: {frequency:F2} Hz, Nota: {note}");
    }

    public void StopRecording()
    {
        if (microphoneDropdown.AvailableMicros())
        {
            string selectedMicro = microphoneDropdown.GetSelectedMicro();

            Microphone.End(selectedMicro);

            Debug.Log("Stop recording from: " + selectedMicro);
        }

    }

    private float GetVolume(float[] data)
    {
        float sum = 0f;
        foreach (float s in data)
            sum += s * s;

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

    private string FrequencyToNoteName(float freq)
    {
        if (freq <= 0f) return "???";

        int midi = Mathf.RoundToInt(12f * Mathf.Log(freq / 440f, 2f) + 69f);
        int octave = (midi / 12) - 1;
        string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        string note = names[midi % 12];
        return $"{note}{octave}";
    }

}