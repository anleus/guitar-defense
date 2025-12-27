using System;
using UnityEngine;

namespace Core.Effects
{
    public class ParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particle;

        private void Start()
        {
            particle.Play();
        }

        private void OnParticleSystemStopped()
        {
            ObjectPoolManager.ReturnObjectToPool(gameObject, ObjectPoolManager.PoolType.ParticleSystem);
        }
    }
}