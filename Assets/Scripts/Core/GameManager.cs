using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using FMODUnity;
using JetBrains.Annotations;
using Models;
using Models.Enums;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public GuitarPattern currentPattern;

        [Header("Configuration")] 
        [SerializeField] private int lives = 3;

        [SerializeField] private int score;
        private const int ScoreGap = 25;
        [SerializeField] private float spawnRate;

        [Header("UI")] 
        [SerializeField] private GameObject tutorialUI;

        [Header("FMOD events")] 
        [SerializeField] private EventReference enemyKillEvent;
        [SerializeField] private EventReference enemyDamageEvent;
        [SerializeField] private StudioEventEmitter backgroundMusicEmitter;
        

        private void OnEnable()
        {
            GameEvents.OnEnemyKilled += AddScore;
            GameEvents.OnLinkedEnemyRequest += HandleLinkedEnemy;
            GameEvents.OnEnemyDamage += ReduceHealth;
        }

        private void OnDisable()
        {
            GameEvents.OnEnemyKilled -= AddScore;
            GameEvents.OnLinkedEnemyRequest -= HandleLinkedEnemy;
            GameEvents.OnEnemyDamage -= ReduceHealth;
        }

        private void Start()
        {
            PlayerPrefs.DeleteKey("tutorial");
            if (CheckTutorial()) StartCoroutine(Game());
            UIEvents.RefreshPatternData(currentPattern.patternName, currentPattern.patternImage);
        }

        private bool CheckTutorial()
        {
            if (!PlayerPrefs.HasKey("tutorial") || PlayerPrefs.GetInt("tutorial") == 0)
            {
                tutorialUI.SetActive(true);
                return false;
            }

            tutorialUI.SetActive(false);
            return true;
        }

        public void StartGameAfterTutorial()
        {
            PlayerPrefs.SetInt("tutorial", 1);
            tutorialUI.SetActive(false);
            StartCoroutine(Game());
        }

        private IEnumerator Game()
        {
            backgroundMusicEmitter.Play();
            
            yield return new WaitForSeconds(1f);
            
            yield return InitialSequence();
            
            yield return Loop();
        }

        private IEnumerator InitialSequence()
        {
            yield return new WaitForSeconds(1f);

            foreach (var guitarString in currentPattern.guitarStrings)
            {
                foreach (var note in guitarString.notes)
                {
                    var fretRelativeIndex = currentPattern.GetIndexFromFret(note.fret);
                    var enemyNoteInfo = new EnemyNoteInfo(
                        note.name, guitarString.stringNum, note.fret, fretRelativeIndex, guitarString.color);
                    GameEvents.SpawnInitialEnemies(enemyNoteInfo);

                    yield return new WaitForSeconds(2.5f);
                }
            }
        }

        private IEnumerator Loop()
        {
            var interval = 2.5f;
            const float minInterval = 1f;
            var lastThreshold = 0;
            
            while (lives > 0)
            {
                var enemyNoteInfo = GetEnemyNoteInfo();
                
                GameEvents.SpawnEnemy(enemyNoteInfo);
                
                var threshold = score / ScoreGap;
                if (threshold > lastThreshold)
                {
                    lastThreshold = threshold;
                    interval = Mathf.Max(minInterval, interval - 0.25f);
                    
                }

                yield return new WaitForSeconds(interval);
            }
        }

        private EnemyNoteInfo GetEnemyNoteInfo()
        {
            var maxString = Math.Max(3 + (score / ScoreGap), 6);
            
            var (note, stringNum) = currentPattern.GetNote(maxString);
            var fretRelativeIndex = currentPattern.GetIndexFromFret(note.fret);
            return new EnemyNoteInfo(note.name, stringNum, note.fret, fretRelativeIndex,
                currentPattern.GetStringColor(stringNum));
        }

        private void HandleLinkedEnemy(LinkedEnemyRequest request)
        {
            var linkedEnemiesNoteInfo = new List<EnemyNoteInfo>();
            var lastUsedString = request.OriginString;
            var lastUsedFret = request.OriginFret;

            for (var i = 0; i < request.LinkedNum; i++)
            {
                var (note, stringNum) = currentPattern.GetNonRepeatedNote(lastUsedString, lastUsedFret);
                lastUsedString = stringNum;
                lastUsedFret = note.fret;

                var fretRelativeIndex = currentPattern.GetIndexFromFret(note.fret);
                var enemyNoteInfo = new EnemyNoteInfo(note.name, stringNum, note.fret, fretRelativeIndex,
                    currentPattern.GetStringColor(stringNum));

                linkedEnemiesNoteInfo.Add(enemyNoteInfo);
            }
            GameEvents.SpawnLinkedEnemies(request.OriginEnemy, linkedEnemiesNoteInfo);
        }
        
        private void AddScore()
        {
            score++;
            UIEvents.RefreshScore(score);
            RuntimeManager.PlayOneShot(enemyKillEvent, transform.position);
        }

        private void ReduceHealth()
        {
            lives = lives - 1 < 0 ? 0 : lives - 1;
            UIEvents.RefreshLivesIndicator(lives);
            RuntimeManager.PlayOneShot(enemyDamageEvent, transform.position);

            if (lives == 0)
            {
                StopAllCoroutines();
                SceneEvents.SceneChange(SceneId.MainMenu);
            }
        }
    }
}