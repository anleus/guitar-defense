using System.Globalization;
using Processors;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(FMODMicroRecorder), typeof(MicrophoneSelector))]
public class AudioManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private MicrophoneSelector microphoneSelector;
    [SerializeField] private FMODMicroRecorder fmodMicroRecorder;
    [SerializeField] private TMP_Text noteText;

    private void Update()
    {
        var freq = fmodMicroRecorder.GetMicroSoundFrequency();
        noteText.text = FrequencyToNote(freq);
    }
    
    public void StartRecording()
    {
        fmodMicroRecorder.StartRecording(microphoneSelector.GetSelectedMicro());
    }

    public void StopRecording()
    {
        fmodMicroRecorder.StopRecording(microphoneSelector.GetSelectedMicro());
        noteText.text = FrequencyToNote(0f);
    }

    private string FrequencyToNote(float frequency)
    {
        if (frequency <= 0f) return "---";

        var midi = Mathf.RoundToInt(12f * Mathf.Log(frequency / 440f, 2f) + 69f);
        var octave = (midi / 12) - 1;
        string[] names = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        var note = names[midi % 12];
        return $"{note}{octave}";
    }
}