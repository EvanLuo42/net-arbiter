using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managedeath : MonoBehaviour
{
    public static managedeath Instance;
    public Vector2 lastPosition;
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
}
