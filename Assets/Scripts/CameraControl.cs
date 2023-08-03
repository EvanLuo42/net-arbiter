using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform playerTransform;
    public Vector3 cameraBias = Vector3.zero;    
        
    //【相机位置函数】
    void CameraPosFun()
    {
        var transform1 = this.transform;
        var position1 = playerTransform.position;
        var position = new Vector3(position1.x, position1.y, -10.0f);
        position += cameraBias;
        transform1.position = position;
    }
    
    // Update is called once per frame
    void Update()
    {
        CameraPosFun();
    }
}
