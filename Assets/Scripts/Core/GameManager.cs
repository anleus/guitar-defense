using System.Collections;
using Events;
using Models;
using UnityEngine;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public GuitarPattern currentPattern;

        [Header("Configuration")]
        [SerializeField] private int lives = 3;
        [SerializeField] private int score;
        [SerializeField] private float spawnRate;

        private void Start()
        {
            StartCoroutine(Game());
            //enviar evento con nombre del patron
        }

        private IEnumerator Game()
        {
            Debug.Log("Start Initial Sequence");
            yield return InitialSequence();
            Debug.Log("End Initial Sequence");
            
            yield return null;
        }

        private IEnumerator InitialSequence()
        {
            yield return new WaitForSeconds(1f);

            foreach (var guitarString in currentPattern.guitarStrings)
            {
                foreach (var note in guitarString.notes)
                {
                    var fretRelativeIndex = currentPattern.GetIndexFromFret(note.fret);
                    var enemyNoteInfo = new EnemyNoteInfo(note.name, note.fret, fretRelativeIndex, guitarString.color);
                    GameEvents.SpawnInitialEnemies(enemyNoteInfo);
                    
                    yield return new WaitForSeconds(1.5f);
                }
            }
        }
    }
}
