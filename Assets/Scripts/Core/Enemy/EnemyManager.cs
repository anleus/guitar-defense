using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using Models;
using UnityEngine;

namespace Core.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyPrefab;
        public List<EnemyType> enemyTypes;
        [SerializeField] private List<Transform> spawnPoints;

        private void OnEnable()
        {
            GameEvents.OnSpawnInitialEnemies += SpawnInitialEnemy;
            GameEvents.OnSpawnEnemy += SpawnEnemy;
            GameEvents.OnSpawnLinkedEnemies += ManageLinkedEnemies;
        }

        private void OnDisable()
        {
            GameEvents.OnSpawnInitialEnemies -= SpawnInitialEnemy;
            GameEvents.OnSpawnEnemy -= SpawnEnemy;
            GameEvents.OnSpawnLinkedEnemies -= ManageLinkedEnemies;
        }

        private void SpawnInitialEnemy(EnemyNoteInfo noteInfo)
        {
            var enemyType = enemyTypes[0];
            var spawnPoint = spawnPoints[noteInfo.FretRelativeIndex];
            
            var enemy = ObjectPoolManager.SpawnObject(enemyPrefab, spawnPoint.position,  spawnPoint.rotation);
            enemy.Setup(enemyType, noteInfo);
        }

        private void SpawnEnemy(EnemyNoteInfo noteInfo)
        {
            var pickedType = PickType();
            Debug.Log(pickedType.name);
            var spawnPoint = spawnPoints[noteInfo.FretRelativeIndex];
            
            var enemy = ObjectPoolManager.SpawnObject(enemyPrefab, spawnPoint.position,  spawnPoint.rotation);
            enemy.Setup(pickedType, noteInfo);
        }

        private void ManageLinkedEnemies(EnemyController parent, List<EnemyNoteInfo> noteInfos)
        {
            StartCoroutine(SpawnLinkedEnemies(parent, noteInfos));
        }

        private IEnumerator SpawnLinkedEnemies(EnemyController parent, List<EnemyNoteInfo> noteInfos)
        {
            var enemyType = enemyTypes[1];
            var previous = parent;
            
            foreach (var noteInfo in noteInfos)
            {
                yield return new WaitForSeconds(0.75f);
                
                var spawnPoint = spawnPoints[noteInfo.FretRelativeIndex];
                var enemy = ObjectPoolManager.SpawnObject(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                
                enemy.SetupLinkedChild(enemyType, noteInfo, previous);
                previous = enemy;
            }
        }

        private EnemyType PickType()
        {
            var total = enemyTypes.Sum(e => e.spawnProbability);
            if (total <= 0.0) return enemyTypes[0];
            
            var randomValue = Random.value * total;
            var cumulative  = 0.0;
            
            foreach (var enemyType in enemyTypes)
            {
                cumulative += enemyType.spawnProbability;
                if (randomValue <= cumulative)
                {
                    return enemyType;
                }
            }
            return enemyTypes.First();
        }
    }
}
