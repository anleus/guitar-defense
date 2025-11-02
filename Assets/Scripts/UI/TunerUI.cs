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
        [SerializeField] private RectTransform indicator;
        [SerializeField] private List<RectTransform> notes;
        
        [Header("Notes")]
        [SerializeField] private TMP_Text noteText;
        [SerializeField] private Image up;
        [SerializeField] private Image down;
        
        private bool visible;
        private int currentString;
        private Coroutine refreshCoroutine;

        public void ToggleUI()
        {
            var toggleAnim = LeanTween.moveX(container, visible ? -375f : 0, 0.75f);
            toggleAnim.setOnComplete(() =>
            {
                visible = !visible;
                UIEvents.ToggleTuner(visible);
                Reset();
            });
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

        private void Reset()
        {
            currentString = 0;
            noteText.text = "-";
            up.fillAmount = 0f;
            down.fillAmount = 0f;

            indicator.position = new Vector3(indicator.position.x, notes[currentString].position.y, 0f);
        }

        private void UpdateUI(string note, float deviation)
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

        private void MoveIndicatorToNextPosition()
        {
            if (!visible) return;
            
            currentString++;
            LeanTween.moveY(indicator, notes[currentString].position.y, 1f);
        }

        private void OnEnable()
        {
            AudioEvents.OnTuningProgress += UpdateUI;
            AudioEvents.OnStringTuned += MoveIndicatorToNextPosition;
        }

        private void OnDisable()
        {
            AudioEvents.OnTuningProgress -= UpdateUI;
            AudioEvents.OnStringTuned -= MoveIndicatorToNextPosition;
        }
    }
}