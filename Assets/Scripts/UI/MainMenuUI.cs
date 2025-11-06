using System;
using UnityEngine;

namespace UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private RectTransform settingsContainer;
        
        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Options()
        {
            LeanTween.moveY(settingsContainer, 575f, 1f);
        }

        public void Back()
        {
            LeanTween.moveY(settingsContainer, 0f, 1f);
        }
    }
}
