using TMPro;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource guitarAudioSource;
    [SerializeField] private MicrophoneDropdown microphoneDropdown;
    [SerializeField] private TMP_Text noteText;
    
    private MicrophoneProcessor micProcessor;

    private void Start()
    {
        micProcessor = new MicrophoneProcessor(44100, 4096);
    }

    private void Update()
    {
        if (micProcessor.IsRecording)
        {
            var note = micProcessor.Process(microphoneDropdown.GetSelectedMicro());
            noteText.text = note;
        }
    }

    public void StartRecording()
    {
        micProcessor.StartRecording(microphoneDropdown.GetSelectedMicro(), guitarAudioSource);
    }

    public void StopRecording()
    {
        micProcessor.StopRecording(microphoneDropdown.GetSelectedMicro());
    }
    
}