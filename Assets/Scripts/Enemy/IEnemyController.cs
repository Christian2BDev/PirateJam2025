using UnityEngine;

namespace Enemy
{
    public interface IEnemyController
    {
        Vector3 PlayerLocation { get; }
        float AggroRange { get; }
        EnemyType EnemyType { get; }
    }
}