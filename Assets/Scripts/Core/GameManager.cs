using System.Collections;
using System.Collections.Generic;
using Events;
using JetBrains.Annotations;
using Models;
using Models.Enums;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public GuitarPattern currentPattern;

        [Header("Configuration")] [SerializeField]
        private int lives = 3;

        [SerializeField] private int score;
        [SerializeField] private float spawnRate;

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
            UIEvents.RefreshPatternData(currentPattern.patternName, currentPattern.patternImage);
            StartCoroutine(Game());
        }

        private IEnumerator Game()
        {
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

                    yield return new WaitForSeconds(1.5f);
                }
            }
        }

        private IEnumerator Loop()
        {
            while (lives > 0)
            {
                var (note, stringNum) = currentPattern.GetNote();
                var fretRelativeIndex = currentPattern.GetIndexFromFret(note.fret);
                var enemyNoteInfo = new EnemyNoteInfo(note.name, stringNum, note.fret, fretRelativeIndex,
                    currentPattern.GetStringColor(stringNum));
                GameEvents.SpawnEnemy(enemyNoteInfo);
                
                yield return new WaitForSeconds(2.5f);
            }
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
        }

        private void ReduceHealth()
        {
            lives = lives - 1 < 0 ? 0 : lives - 1;
            UIEvents.RefreshLivesIndicator(lives);

            if (lives == 0)
            {
                StopAllCoroutines();
                SceneEvents.SceneChange(SceneId.MainMenu);
            }
        }
    }
}