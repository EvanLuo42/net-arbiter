using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class Attackdone : MonoBehaviour
{
    public GameObject visual;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void attackdone()
    {
        visual.GetComponent<PlayerAnimator>().attackdone();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
