using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadcheck : MonoBehaviour
{
    private PlayerController playerController;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
