using Processors;
using TMPro;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private AudioSource guitarAudioSource;
    [SerializeField] private MicrophoneSelector microphoneSelector;
    [SerializeField] private TMP_Text noteText;

    [Header("Settings")] 
    [SerializeField] private int sampleRate = 44100;
    [SerializeField] private int sampleWindow = 4096;
    
    private PitchProcessor pitchProcessor;
    private AudioClip microphoneClip;
    private float[] audioSamples;

    private bool isRecording;
    private string selectedMicName;

    private void Start()
    {
        audioSamples = new float[sampleWindow];
        pitchProcessor = new PitchProcessor(sampleRate);
    }

    private void Update()
    {
        if (!isRecording) return;

        var micPos = Microphone.GetPosition(selectedMicName) - sampleWindow;
        if (micPos < 0)
        {
            noteText.text = "---";
            return;
        }

        microphoneClip.GetData(audioSamples, micPos);

        var frequency = pitchProcessor.Process(audioSamples);

        noteText.text = frequency;
    }

    public void StartRecording()
    {
        selectedMicName = microphoneSelector.GetSelectedMicro().Name;

        microphoneClip = Microphone.Start(selectedMicName, true, 1, sampleRate);
        
        while (!(Microphone.GetPosition(selectedMicName) > 0))
        {
        }

        guitarAudioSource.clip = microphoneClip;
        guitarAudioSource.loop = true;
        guitarAudioSource.Play();
        
        isRecording = true;
    }

    public void StopRecording()
    {
        Microphone.End(selectedMicName);

        isRecording = false;
    }
}