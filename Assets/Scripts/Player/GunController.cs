using UnityEngine;
using Player;
using Shared;
using Sound;

public class GunController : MonoBehaviour
{
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    
    ObjectPooler _objectPooler;

    public int damage;
    void Start()
    {
        UserInput.Main.OnPlayerFire += OnFire;
        _objectPooler = ObjectPooler.Instance;
        _objectPooler.RegisterPool<Bullet>(bulletPrefab);
    }

    private void OnFire() { 
        Quaternion spawnRotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        var bullet = _objectPooler.SpawnFromPool<Bullet>(bulletSpawnPoint.transform.position , spawnRotation);
        bullet.damage = damage;
        bullet._canHit = CanHit.Enemys;
        
        bullet.GetComponent<Bullet>().damage = damage;
        AudioManager.Play(SoundType.Shoot);
    }
}
