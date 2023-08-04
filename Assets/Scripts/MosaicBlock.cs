using System;
using System.Linq;
using UnityEngine;

public class MosaicBlock : MonoBehaviour
{
    public GameObject bulletShooter;
    private BulletShooter _bulletShooterController;

    public float collisionRadius;

    private void Awake()
    {
        _bulletShooterController = bulletShooter.GetComponent<BulletShooter>();
    }

    private void Update()
    {
        // var bullet = Physics2D.OverlapBox(transform.position, new Vector2(20, 20), 0, BulletMask);
        // if (!bullet) return;
        // var bulletController = bullet.GetComponent<Bullet>();
        // bulletController.destroy = true;
        // Destroy(this);
        foreach (var bullet in _bulletShooterController
                     .Bullets.Values
                     .Where(bullet => 
                         Vector3.Distance(transform.position, bullet.Item1.transform.position) < collisionRadius)
                     .Where(bullet => bullet.Item2.destroyable))
        {
            bullet.Item2.destroy = true;
            Destroy(gameObject);
        }
    }
}
