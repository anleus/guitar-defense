using Core.Enemy;

namespace Models
{
    public class LinkedEnemyRequest
    {
        public readonly EnemyController OriginEnemy;
        public readonly int OriginString;
        public readonly int OriginFret;
        public readonly int LinkedNum;

        public LinkedEnemyRequest(EnemyController originEnemy, int originString, int originFret, int linkedNum)
        {
            OriginEnemy = originEnemy;
            OriginString = originString;
            OriginFret = originFret;
            LinkedNum = linkedNum;
        }
    }
}