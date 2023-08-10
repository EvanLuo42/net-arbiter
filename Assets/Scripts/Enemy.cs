using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public releaseEnemies Father;
    public bool QiSiHuiSheng;
    public GameObject Pattern;
    // Start is called before the first frame update
    public void InstantiatePattern()
    {
        Instantiate(Pattern,new Vector2(transform.position.x + 1.09f, transform.position.y + 0.92f),new Quaternion(0,0,0,0),transform);
    }
    public void death()
    {
        if (QiSiHuiSheng)UpdatePositionToFatherObject();
        Destroy(gameObject);
    }
    public void UpdatePositionToFatherObject()
    {
        Father.SetChildPositionAndRotationAndComBackForLife(transform.position, transform.rotation);
    }
    void Awake()
    {
        Father = GetComponentInParent<releaseEnemies>();
    }
    // Update is called once per frame
}
