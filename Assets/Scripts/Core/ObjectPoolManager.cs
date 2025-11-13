using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private GameObject emptyHolder;

        private static GameObject gameObjectEmpty;
        private static GameObject particleSystemEmpty;
        
        private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
        private static Dictionary<GameObject, GameObject> cloneToPrefabMap;
        
        public enum PoolType {
            GameObjects,
            ParticleSystem
        }
        public static PoolType PoolingType;

        private void Awake()
        {
            objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
            cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

            SetupEntities();
        }

        private void SetupEntities()
        {
            emptyHolder = new GameObject("Object Pools");
            
            gameObjectEmpty = new GameObject("Game Objects");
            gameObjectEmpty.transform.SetParent(emptyHolder.transform);
            
            particleSystemEmpty = new GameObject("Particle Effects");
            particleSystemEmpty.transform.SetParent(emptyHolder.transform);
        }

        private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () => CreateGameObject(prefab, pos, rot, poolType),
                actionOnGet: OnGetGameObject,
                actionOnRelease: OnReleaseGameObject,
                actionOnDestroy: OnDestroyGameObject);
            
            objectPools.Add(prefab, pool);
        }

        private static GameObject CreateGameObject(GameObject prefab, Vector3 pos, Quaternion rot,
            PoolType poolType = PoolType.GameObjects)
        {
            prefab.SetActive(false);
            
            var obj = Instantiate(prefab, pos, rot);
            
            prefab.SetActive(true);

            var parent = SetParentObject(poolType);
            obj.transform.SetParent(parent.transform);
            
            return obj;
        }

        private static void OnGetGameObject(GameObject gameObject)
        {
            //optional logic
        }

        private static void OnReleaseGameObject(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }

        private static void OnDestroyGameObject(GameObject gameObject)
        {
            cloneToPrefabMap.Remove(gameObject);
        }

        private static GameObject SetParentObject(PoolType poolType)
        {
            switch (poolType)
            {
                case PoolType.GameObjects:
                    return gameObjectEmpty;
                case PoolType.ParticleSystem:
                    return particleSystemEmpty;
                default:
                    return null;
            }
        }

        private static T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation,
            PoolType poolType = PoolType.GameObjects) where T : Object
        {
            if (!objectPools.ContainsKey(objectToSpawn))
            {
                CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
            }

            var obj = objectPools[objectToSpawn].Get();

            if (obj != null)
            {
                cloneToPrefabMap.TryAdd(obj, objectToSpawn);
                
                obj.transform.position = spawnPos;
                obj.transform.rotation = spawnRotation;
                obj.SetActive(true);

                if (typeof(T) == typeof(GameObject))
                {
                    return obj as T;
                }
                
                var component = obj.GetComponent<T>();
                if (component == null)
                {
                    Debug.LogError($"Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
                    return null;
                }
                return component;
            }
            return null;
        }

        public static T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
        {
            return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
        }
        
        public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
        {
            return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
        }

        public static void ReturnObjectToPool(GameObject objectToReturn, PoolType poolType = PoolType.GameObjects)
        {
            if (cloneToPrefabMap.TryGetValue(objectToReturn, out var prefab))
            {
                var parentObject = SetParentObject(poolType);

                if (objectToReturn.transform.parent != parentObject.transform)
                {
                    objectToReturn.transform.SetParent(parentObject.transform);
                }

                if (objectPools.TryGetValue(prefab, out var pool))
                {
                    pool.Release(objectToReturn);
                }
            }
            else
            {
                Debug.LogWarning("Trying to return an object that is not pooled: " + objectToReturn.name);
            }
        }
    }
}