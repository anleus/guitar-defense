using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneMeasurer : MonoBehaviour
{
    [SerializeField] private Image volumeIndicator;
    [SerializeField] private AudioSource audioSource;

    float[] samples = new float[1024];

    private void Update()
    {
        audioSource.GetOutputData(samples, 0); // Canal 0 = izquierdo
        float sum = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i]; // cuadrado de la amplitud
        }

        float rms = Mathf.Sqrt(sum / samples.Length); // Root Mean Square
        float db = 20 * Mathf.Log10(rms); // Decibeles aproximados

        Debug.Log($"Volumen RMS: {rms:F3} | dB: {db:F1}");

        volumeIndicator.fillAmount = db / 100f;
    }
}
