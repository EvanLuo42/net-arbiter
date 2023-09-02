using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindWithTag("management") == null) return;
        Destroy(GameObject.FindWithTag("management"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
