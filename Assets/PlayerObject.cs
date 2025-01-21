using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObject", menuName = "Objects/PlayerObject")]
public class PlayerObject : ScriptableObject
{
    public int MaxHP;
    public int MoveSpeed;
    public int BulletSpeed;
    public int Damage;
    public int energy;

}
