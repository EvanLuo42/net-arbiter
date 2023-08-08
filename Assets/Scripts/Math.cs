using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Math : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static Vector2 Direction(Vector2 input)
    {
        float z = Mathf.Sqrt(input.x * input.x + input.y * input.y);
        return new Vector2(input.x / z, input.y / z);
    }

    
}
