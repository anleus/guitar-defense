using System.Linq;
using UnityEngine;

public class MicrophoneProcessor
{
    private int sampleRate;
    private int sampleWindow;
    private float[] audioSamples;
    private AudioClip microphoneClip;

    public bool IsRecording { get; private set; }

    public MicrophoneProcessor(int sampleRate, int sampleWindow)
    {
        this.sampleRate = sampleRate;
        this.sampleWindow = sampleWindow;
        this.audioSamples = new float[sampleWindow];
    }

    public void StartRecording(string micName, AudioSource audioSource)
    {
        microphoneClip = Microphone.Start(micName, true, 1, sampleRate);
        audioSource.clip = microphoneClip;
        audioSource.loop = true;
        
        while (!(Microphone.GetPosition(micName) > 0)) { }

        audioSource.Play();
        IsRecording = true;
        /*if (!microphoneDropdown.AvailableMicros())
        {
            Debug.LogWarning("No available devices");
        }
        else
        {
            microphoneClip = Microphone.Start(microphoneDropdown.GetSelectedMicro(), true, 10, sampleRate);
            audioSamples = new float[sampleWindow];
        }*/
    }

    public void StopRecording(string micName)
    {
        Microphone.End(micName);
        IsRecording = false;
        /*if (microphoneDropdown.AvailableMicros())
        {
            string selectedMicro = microphoneDropdown.GetSelectedMicro();

            Microphone.End(selectedMicro);

            Debug.Log("Stop recording from: " + selectedMicro);
        }*/
    }

    public string Process(string micName)
    {
        var micSamplePosition = Microphone.GetPosition(micName) - sampleWindow;
        if (micSamplePosition < 0) return "---";

        microphoneClip.GetData(audioSamples, micSamplePosition);
        var volume = GetVolume(audioSamples);
        if (volume < 0.01f) return "---";

        var frequency = DetectPitch(audioSamples);
        if (frequency is < 60f or > 1300f) return "---";
        
        Debug.Log(frequency);

        return FrequencyToNoteName(frequency);
    }

    private float GetVolume(float[] data)
    {
        var sum = data.Sum(s => s * s);

        return Mathf.Sqrt(sum / data.Length);
    }

    private float DetectPitch(float[] data)
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
        return sampleRate / bestLag;
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