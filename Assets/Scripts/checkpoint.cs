using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void updatetomanager()
    {
        managedeath.Instance.lastPosition = new Vector2(transform.position.x,transform.position.y + 0.1f);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
