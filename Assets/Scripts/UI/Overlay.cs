using System.Collections.Generic;
using Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Overlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text patternText;
        [SerializeField] private Image patterImageContainer;
        
        [SerializeField] private TMP_Text lastPlayedNoteText;
        
        [SerializeField] private List<Image> livesContainer;

        private void OnEnable()
        {
            UIEvents.OnRefreshPatternData += SetPatternData;
            UIEvents.OnRefreshScore += SetScore;
            UIEvents.OnRefreshLivesIndicator += SetLivesIndicatorStatus;
            
            AudioEvents.OnNoteDetected += SetLastPlayedNote;
        }

        private void OnDisable()
        {
            UIEvents.OnRefreshPatternData -= SetPatternData;
            UIEvents.OnRefreshScore -= SetScore;
            UIEvents.OnRefreshLivesIndicator -= SetLivesIndicatorStatus;
            
            AudioEvents.OnNoteDetected -= SetLastPlayedNote;
        }

        private void SetPatternData(string patternName, Sprite image)
        {
            patternText.text = patternName;
            patterImageContainer.sprite = image;
        }

        private void SetScore(int score)
        {
            scoreText.text = score.ToString("D9");
        }

        private void SetLivesIndicatorStatus(int lives)
        {
            livesContainer[lives].color = Color.black;
        }

        private void SetLastPlayedNote(string lastPlayedNote)
        {
            lastPlayedNoteText.text = lastPlayedNote;
        }
    }
}
