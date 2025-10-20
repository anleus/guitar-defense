using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MicrophoneDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown microDropdown;
    private List<string> microOptions = new List<string>();
    private int selectedMicro = -1;

    private void Awake()
    {
        SetMicroOptions();
    }

    public bool AvailableMicros()
    {
        return microOptions.Count > 0;
    }

    public string GetSelectedMicro()
    {
        return microOptions[selectedMicro];
    }

    private void SetMicroOptions()
    {
        microOptions = new List<string>(Microphone.devices);
        microDropdown.AddOptions(microOptions);
        if (microOptions.Count > 0)
        {
            selectedMicro = 0;
            Debug.Log("Default selected index " + selectedMicro + " -> " + microDropdown.options[selectedMicro].text);
        }
    }

    public void OnOptionSelected(int index)
    {
        selectedMicro = index;
        Debug.Log("Selected index " + index + " -> " + microDropdown.options[index].text);
    }
}
