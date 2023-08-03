using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class BulletShooter : MonoBehaviour
{
    public float delay;
    public float interval;
    public float lifetime;
    public GameObject bulletPrefab;
    public float bulletSpeed;
    private Dictionary<float, GameObject> _bullets = new();
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
        
        foreach (var bulletPair in _bullets)
        {
            var bullet = bulletPair.Value.GetComponent<Bullet>();
            if (bullet.destroy || _passedTime - bulletPair.Key > lifetime)
            {
                _bullets.Remove(bulletPair.Key);
                Destroy(bulletPair.Value);
            }

            Vector3 velocity = bullet.direction * bulletSpeed;
            bulletPair.Value.transform.position += velocity * Time.deltaTime;
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newBullet = Instantiate(bulletPrefab, transform, true);
        newBullet.transform.position = transform.position;
        newBullet.GetComponent<Bullet>().direction = Vector2.left;
        _bullets.Add(_passedTime, newBullet);
        _lastShootTime = _passedTime;
    }
}
