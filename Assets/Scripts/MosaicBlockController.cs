using System;
using UnityEngine;

public class MosaicBlockController : MonoBehaviour
{
    public LayerMask BulletMask;
    private void Update()
    {
        var bullet = Physics2D.OverlapBox(transform.position, new Vector2(20, 20), 0, BulletMask);
        if (!bullet) return;
        var bulletController = bullet.GetComponent<Bullet>();
        bulletController.destroy = true;
        Destroy(this);
    }
}
