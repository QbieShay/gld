using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float shootRatio = 0.4f;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    private float shootTime = 0f;

    private void Update()
    {
        shootTime += Time.deltaTime;
    }

    public void Shoot()
    {
        if (shootTime >= shootRatio)
        {
            shootTime = 0f;
            bulletPrefab.Spawn(shootPoint.position, transform.rotation);
        }
    }
}
