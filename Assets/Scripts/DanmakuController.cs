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
            _danmakus[time].Item2.transform.position += velocity * Time.deltaTime; 
        }

        if (!(_passedTime - _lastShootTime > interval)) return;
        
        var newDanmaku = Instantiate(danmakuPrefab, transform, true);
        var newDanmakuController = newDanmaku.GetComponent<Danmaku>();
        newDanmakuController.transform.position = transform.position;
        newDanmakuController.direction = Vector2.left;
        _danmakus.Add(_passedTime, new Tuple<GameObject, Danmaku>(newDanmaku, newDanmakuController));
        _lastShootTime = _passedTime;
    }
}
