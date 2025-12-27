using System;
using System.Collections;
using Events;
using Unity.Cinemachine;
using UnityEngine;
using Utils;

namespace Core.Effects
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private CinemachineCamera cmCamera;
        [SerializeField] private CinemachineBasicMultiChannelPerlin perlinNoise;

        [SerializeField] private float intensity;
        [SerializeField] private float duration;
        
        private Coroutine shakeCoroutine;

        private void OnEnable()
        {
            GameEvents.OnEnemyDamage += ShakeCamera;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyDamage -= ShakeCamera;
        }

        private void ShakeCamera()
        {
            CoroutineUtils.RestartCoroutine(this, ref shakeCoroutine, Shake());
        }

        private IEnumerator Shake()
        {
            perlinNoise.AmplitudeGain = intensity;
            yield return new WaitForSeconds(duration);
            perlinNoise.AmplitudeGain = 0f;
        }
        
        
    }
    
}