using UnityEngine;

[CreateAssetMenu(fileName = "EnemyObject", menuName = "Objects/EnemyObject")]
public class EnemyObject : ScriptableObject
{
    public new string name;
    public GameObject EnemyModel;
    public enemytype Enemytype; 
    public int MaxHP;   
    public int MoveSpeed;
    public int BulletSpeed;
    public int Damage;
    public int AggroRange;
    public int ShootingRange;
    public int AttackSpeed;   
    public enum enemytype
    {
        gun,
        melee,
    }                        
}
