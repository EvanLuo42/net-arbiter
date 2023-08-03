using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class DanmakuController : MonoBehaviour
{
    public float interval;
    public float lifetime;
    public GameObject danmakuPrefab;
    public float danmakuSpeed;
    private Dictionary<float, GameObject> _danmakus = new();
    private float _passedTime;
    private float _lastShootTime;

    private void Update()
    {
        _passedTime += Time.deltaTime;
        foreach (var danmakuPair in _danmakus)
        {
            if (danmakuPair.Value.GetComponent<Danmaku>().destroy || _passedTime - danmakuPair.Key > lifetime)
            {
                _danmakus.Remove(danmakuPair.Key);
                Destroy(danmakuPair.Value);
            }

            var danmaku = danmakuPair.Value;
            Vector3 velocity = danmaku.GetComponent<Danmaku>().direction * danmakuSpeed;
            danmakuPair.Value.transform.position += velocity * Time.deltaTime; 
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newDanmaku = Instantiate(danmakuPrefab, transform, true);
        newDanmaku.GetComponent<Danmaku>().direction = Vector2.left;
        _danmakus.Add(_passedTime, newDanmaku);
        _lastShootTime = _passedTime;
    }
}
