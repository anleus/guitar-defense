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
        }

        private void ResetValues()
        {
            currentString = 0;
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
                currentString = 0;
            }
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