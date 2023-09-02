using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setorigion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("management").GetComponent<managedeath>().lastPosition = new Vector2(-10.56f,-0.36f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
