using UnityEngine;
using Player;

public class GunController : MonoBehaviour
{
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;

    public int damage;
    void Start()
    {
        UserInput.Main.OnPlayerFire += OnFire;

    }

    private void OnFire() { 
        Quaternion spawnRotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        GameObject bullet =  Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, spawnRotation);
        bullet.GetComponent<Bullet>().damage = damage;
    }
}
