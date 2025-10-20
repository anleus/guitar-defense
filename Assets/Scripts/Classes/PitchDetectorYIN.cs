using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchDetectorYIN
{
    private readonly int sampleRate = 44100;
    private readonly int bufferSize = 4096;
    private readonly float threshold = 0.1f;
    private readonly int smoothWindow = 4;

    private readonly Queue<float> lastFrequencies = new Queue<float>();

    public PitchDetectorYIN(int sampleRate = 44100, int bufferSize = 4096, float threshold = 0.1f, int smoothWindow = 4)
    {
        this.sampleRate = sampleRate;
        this.bufferSize = bufferSize;
        this.threshold = threshold;
        this.smoothWindow = smoothWindow;
    }

}
