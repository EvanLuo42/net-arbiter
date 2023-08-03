using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class DanmakuController : MonoBehaviour
{
    public float interval;
    public float lifetime;
    public GameObject danmakuPrefab;
    public float danmakuSpeedX;
    public float danmakuSpeedY;
    private Dictionary<float, GameObject> _danmakus = new();
    private float _passedTime;
    private float _lastShootTime;

    private void Update()
    {
        _passedTime += Time.deltaTime;
        foreach (var danmakuPair in _danmakus)
        {
            if (danmakuPair.Value.GetComponent<Danmaku>().destory || _passedTime - danmakuPair.Key > lifetime)
            {
                _danmakus.Remove(danmakuPair.Key);
                Destroy(danmakuPair.Value);
            }

            var danmukuCurrentPosition = danmakuPair.Value.transform.position;
            danmakuPair.Value.transform.position = new Vector3(danmukuCurrentPosition.x + danmakuSpeedX * Time.deltaTime,
                danmukuCurrentPosition.y + danmakuSpeedY * Time.deltaTime, danmukuCurrentPosition.z);
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newDanmaku = Instantiate(danmakuPrefab, transform, true);
        newDanmaku.transform.position = transform.position;
        _danmakus.Add(_passedTime, newDanmaku);
        _lastShootTime = _passedTime;
    }
}
