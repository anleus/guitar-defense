using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "Enemy configuration", menuName = "ScriptableObjects/Enemy Configuration")]
    public class EnemyType : ScriptableObject
    {
        public int health;
        public float speed;
        public bool linked;
        public int linkedNum = 1;
        public Sprite sprite;

        public double spawnProbability;
    }
}
