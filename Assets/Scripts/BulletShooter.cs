using System;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour
{
    public float delay;
    public float interval;
    public float lifetime;
    public float destroyableTime;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    public readonly Dictionary<float, Tuple<GameObject, Bullet>> Bullets = new();
    private float _passedTime;
    private float _lastShootTime;
    private float _lastUpdateTime;

    private void Update()
    {
        _passedTime += Time.deltaTime;
        if (_passedTime - _lastUpdateTime < delay)
        {
            _lastUpdateTime = Time.deltaTime;
            return;
        }

        var bulletsKeys = new List<float>(Bullets.Keys);
        foreach (var time in bulletsKeys)
        {
            var bulletController = Bullets[time].Item2;
            if (bulletController.destroy || _passedTime - time > lifetime)
            {
                Destroy(Bullets[time].Item1);
                Bullets.Remove(time);
                continue;
            }

            if (_passedTime - time > destroyableTime)
            {
                bulletController.destroyable = true;
            }

            Vector3 velocity = bulletController.direction * bulletSpeed;
            Bullets[time].Item1.transform.position += velocity * Time.deltaTime;
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newBullet = Instantiate(bulletPrefab, transform, true);
        newBullet.transform.position = transform.position;
        var newBulletController = newBullet.GetComponent<Bullet>();
        newBulletController.direction = Vector2.left;
        Bullets.Add(_passedTime, new Tuple<GameObject, Bullet>(newBullet, newBulletController));
        _lastShootTime = _passedTime;
    }
}
