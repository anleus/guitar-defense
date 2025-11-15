using System;
using System.Collections.Generic;
using Core.Enemy;
using Models;
using UnityEngine;

namespace Events
{
    public static class GameEvents
    {

        public static event Action<EnemyNoteInfo> OnSpawnEnemy;
        public static void SpawnEnemy(EnemyNoteInfo enemyNoteInfo)
        {
            OnSpawnEnemy?.Invoke(enemyNoteInfo);
        }

        public static event Action<EnemyNoteInfo> OnSpawnInitialEnemies;
        public static void SpawnInitialEnemies(EnemyNoteInfo enemyNoteInfo)
        {
            OnSpawnInitialEnemies?.Invoke(enemyNoteInfo);
        }
        
        public static event Action<EnemyController, List<EnemyNoteInfo>> OnSpawnLinkedEnemies;
        public static void SpawnLinkedEnemies(EnemyController parent, List<EnemyNoteInfo> enemyNoteInfos)
        {
            OnSpawnLinkedEnemies?.Invoke(parent, enemyNoteInfos);
        }
        
        public static event Action<LinkedEnemyRequest> OnLinkedEnemyRequest;
        public static void SpawnLinkedEnemyRequest(LinkedEnemyRequest request) {
            OnLinkedEnemyRequest?.Invoke(request);
        }

        public static event Action OnEnemyKilled;

        public static void EnemyKilled()
        {
            OnEnemyKilled?.Invoke();
        }

        public static event Action OnEnemyDamage;
        public static void EnemyDamage()
        {
            OnEnemyDamage?.Invoke();
        }
    }
}
