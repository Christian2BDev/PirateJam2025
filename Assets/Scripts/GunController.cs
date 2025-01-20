using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GunController : MonoBehaviour
{
    private PlayerControls _playerControls;
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public float damage;
    void Start() {
        _playerControls = new PlayerControls();
    }
    
    private void OnFire() { 
        Quaternion spawnRotation = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        GameObject bullet =  Instantiate(bulletPrefab, bulletSpawnPoint.transform.position, spawnRotation);
        bullet.GetComponent<Bullet>().damage = damage;
    }
}
