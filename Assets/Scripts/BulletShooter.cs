using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BulletShooter : MonoBehaviour
{
    public float delay;
    public float interval;
    public float lifetime;
    public float destroyableTime;
    public GameObject bulletPrefab;
    public GameObject player;
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

        var position = transform.position;
        var newBullet = Instantiate(bulletPrefab, position, Quaternion.identity, transform);
        var newBulletController = newBullet.GetComponent<Bullet>();
        newBulletController.direction = (Quaternion.AngleAxis(new Random().Next(-80, 80), Vector3.up) * (player.transform.position - position).normalized).normalized;
        Bullets.Add(_passedTime, new Tuple<GameObject, Bullet>(newBullet, newBulletController));
        _lastShootTime = _passedTime;
    }
}
