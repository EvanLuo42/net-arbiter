using System;
using System.Collections.Generic;
using UnityEngine;

public class DanmakuController : MonoBehaviour
{
    public float interval;
    public float lifetime;
    public GameObject danmakuPrefab;
    public float danmakuSpeed;
    public Vector2 direction;
    public Sprite[] sprites;
    private Dictionary<float, Tuple<GameObject, Danmaku>> _danmakus = new();
    private float _passedTime;
    private float _lastShootTime;

    private void Update()
    {
        _passedTime += Time.deltaTime;
        var danmakusKeys = new List<float>(_danmakus.Keys);
        foreach (var time in danmakusKeys)
        {
            if (_danmakus[time].Item2.destroy || _passedTime - time > lifetime)
            {
                Destroy(_danmakus[time].Item1);
                _danmakus.Remove(time);
                continue;
            }
            Vector3 velocity = _danmakus[time].Item2.direction * danmakuSpeed;
            _danmakus[time].Item1.transform.position += velocity * Time.deltaTime;
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newDanmaku = Instantiate(danmakuPrefab, transform.position, Quaternion.identity, transform);
        var sprite = newDanmaku.GetComponent<SpriteRenderer>();
        sprite.sprite = sprites[new System.Random().Next(0, sprites.Length - 1)];
        var newDanmakuController = newDanmaku.GetComponent<Danmaku>();
        newDanmakuController.direction = direction;
        _danmakus.Add(_passedTime, new Tuple<GameObject, Danmaku>(newDanmaku, newDanmakuController));
        _lastShootTime = _passedTime;
    }
}
