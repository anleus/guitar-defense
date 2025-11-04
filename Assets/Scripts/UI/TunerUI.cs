using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI
{
    public class TunerUI : MonoBehaviour
    {
        [Header("Strings")] 
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform stringIndicator;
        [SerializeField] private Image indicatorImage;
        [SerializeField] private List<RectTransform> notes;

        [Header("Notes")] [SerializeField] private TMP_Text noteText;
        [SerializeField] private Image up;
        [SerializeField] private Image down;

        [Header("Tunings")] 
        [SerializeField] private List<Image> tuningIndicators;

        [SerializeField] private Button toggleButton;

        private bool visible;
        private int currentString;
        private Coroutine refreshCoroutine;

        private const int Tunings = 2;
        private int currentTunings = -1;
        
        private FMOD.Studio.EventInstance instance;

        private void Start()
        {
            instance = FMODUnity.RuntimeManager.CreateInstance("event:/tuning/check");
        }

        public void ToggleUI()
        {
            var toggleAnim = LeanTween.moveX(container, visible ? -375f : 0, 0.75f);
            toggleAnim.setOnComplete(OnToggle);
        }

        private void OnToggle()
        {
            ResetValues();
            visible = !visible;
            UIEvents.TunerVisible(visible);

            if (visible)
            {
                ManageToggleButton(false);
                ManageTuningIndicator();
            }
        }

        private void ResetValues()
        {
            currentString = 0;
            currentTunings = -1;
            noteText.text = "-";
            up.fillAmount = 0f;
            down.fillAmount = 0f;

            stringIndicator.position = new Vector3(stringIndicator.position.x, notes[currentString].position.y, 0f);
            CoroutineUtils.RestartCoroutine(this, ref refreshCoroutine, Refresh(), false);
        }

        private void UpdateNoteIndicator(string note, float deviation)
        {
            if (!visible) return;

            CoroutineUtils.RestartCoroutine(this, ref refreshCoroutine, Refresh());

            noteText.text = note;

            var normalizedDeviation = Mathf.Clamp01(Mathf.Abs(deviation) / 50f);

            if (deviation > 0)
            {
                up.fillAmount = normalizedDeviation;
                down.fillAmount = 0;
            }
            else
            {
                down.fillAmount = normalizedDeviation;
                up.fillAmount = 0f;
            }
        }

        private IEnumerator Refresh()
        {
            while (visible)
            {
                yield return new WaitForSeconds(1.5f);
                noteText.text = "-";
                up.fillAmount = 0f;
                down.fillAmount = 0f;
            }
        }

        private void MoveIndicatorToNextPosition()
        {
            if (!visible) return;

            CheckStatus();

            instance.start();
            instance.release();
            
            indicatorImage.color = Color.limeGreen;
            var indicatorAnim = LeanTween.moveY(stringIndicator, notes[currentString].anchoredPosition.y, 0.5f);
            indicatorAnim.setOnComplete(() => indicatorImage.color = Color.black);
        }

        private void CheckStatus()
        {
            if (currentString < notes.Count - 1)
            {
                currentString++;
            }
            else
            {
                if (currentTunings < Tunings)
                {
                    currentTunings++;
                    currentString = 0;
                    ManageTuningIndicator();
                }
                else
                {
                    ManageToggleButton(true);
                    ToggleUI();
                }
            }
        }

        private void ManageTuningIndicator()
        {
            if (currentTunings == -1)
            {
                foreach (var indicator in tuningIndicators) indicator.color = Color.white;
            }
            else
            {
                tuningIndicators[currentTunings].color = Color.limeGreen;
            }
        }

        private void ManageToggleButton(bool interactable)
        {
            toggleButton.interactable = interactable;
        }

        private void OnEnable()
        {
            AudioEvents.OnTuningProgress += UpdateNoteIndicator;
            AudioEvents.OnStringTuned += MoveIndicatorToNextPosition;
        }

        private void OnDisable()
        {
            AudioEvents.OnTuningProgress -= UpdateNoteIndicator;
            AudioEvents.OnStringTuned -= MoveIndicatorToNextPosition;
        }
    }
}