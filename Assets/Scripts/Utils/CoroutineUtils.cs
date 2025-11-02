using System.Collections;
using UnityEngine;

namespace Utils
{
    public static class CoroutineUtils
    {
        public static Coroutine RestartCoroutine(MonoBehaviour mono, ref Coroutine currentCoroutine,
            IEnumerator routine, bool startNew = true)
        {
            if (currentCoroutine != null)
            {
                mono.StopCoroutine(currentCoroutine);
                currentCoroutine = null;
            }
            if (startNew) currentCoroutine = mono.StartCoroutine(routine);
            
            return currentCoroutine;
        }
    }
}