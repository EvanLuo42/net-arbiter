using System.Collections.Generic;
using UnityEngine;

public class Danmaku : MonoBehaviour
{
    public bool destroy;
    public Vector2 direction;
    [SerializeField] public List<DanmakuType> danmakuTypes;
    
    public enum DanmakuType
    {
        DisableDash, DisableDoubleJump
    }
}
