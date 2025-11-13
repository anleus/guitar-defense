using System.Collections.Generic;
using Events;
using Models;
using UnityEngine;
using Random = System.Random;

namespace Core.Enemies
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyPrefab;
        public List<EnemyType> enemyTypes;
        [SerializeField] private List<Transform> spawnPoints;

        private void OnEnable()
        {
            GameEvents.OnSpawnInitialEnemies += SpawnInitialEnemy;
        }

        private void OnDisable()
        {
            GameEvents.OnSpawnInitialEnemies -= SpawnInitialEnemy;
        }

        private void SpawnInitialEnemy(EnemyNoteInfo noteInfo)
        {
            var enemyType = enemyTypes[0];
            var spawnPoint = spawnPoints[noteInfo.FretRelativeIndex];
            
            var enemy = ObjectPoolManager.SpawnObject(enemyPrefab, spawnPoint.position,  spawnPoint.rotation);
            enemy.Setup(enemyType, noteInfo);
        }

        private void SpawnEnemy()
        {
            var pickedType = PickType();

            //var enemy = ObjectPoolManager.SpawnObject(enemyPrefab);

        }

        private EnemyType PickType()
        {
            var value = UnityEngine.Random.value;

            var cumulative  = 0.0;
            foreach (var enemyType in enemyTypes)
            {
                cumulative += enemyType.spawnProbability;
                if (value < cumulative)
                {
                    return enemyType;
                }
            }
            return null;
        }
    }
}
