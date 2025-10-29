using System;
using UnityEngine;

namespace Events
{
    public static class GameEvents
    {
        public static event Action OnGameLoopStart;
        public static void GameLoopStart()
        {
            OnGameLoopStart?.Invoke();
        }
        
        public static event Action OnGameLoopPause;
        public static void GameLoopPause()
        {
            OnGameLoopPause?.Invoke();
        }
        
        public static event Action OnGameLoopStop;
        public static void GameLoopStop()
        {
            OnGameLoopStop?.Invoke();
        }
    }
}
