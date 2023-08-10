using System.Collections;
using System.Collections.Generic;
using TarodevController;
using UnityEngine;

public class PatternOnEnemy : MonoBehaviour
{
    public string FirstToClipOn;
    public string SecondToClipOn;
    public GameObject LatterGameobject;
    public bool StartOrNot;
    private bool DetectDaoLe;
    public GameObject Arrow;
    public GameObject Player;
    // Start is called before the first frame update
    private void Awake()
    {
        Player = GameObject.FindWithTag("player");
    }
    private bool DetectRightKey()
    {
        if (SecondToClipOn != "")
        {
            if (Input.GetKey(FirstToClipOn) && Input.GetKey(SecondToClipOn))
            {
                DisappearAndWhat();
                return true;
            }
        }
        else
        {
            if (Input.GetKey(FirstToClipOn))
            {
                DisappearAndWhat();
                return true;
            }
        }
        return false;
    }
    private void DisappearAndWhat()
    {
        GetComponent<SpriteRenderer>().color = new Color32(0,0,0,0);
        Arrow.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0, 0);
        StartOrNot = false;
        DetectDaoLe = false;
        //Invoke("DestroyInAMinute", 0.5f);
    }
    // Update is called once per frame
    void Update()
    {
        if (StartOrNot) DetectDaoLe = DetectRightKey();
        if (DetectDaoLe)
        {
            if (LatterGameobject == null)
            {
                Player.GetComponent<PlayerController>()._FinishPattern = true;
                return;
            }
            LatterGameobject.GetComponent<PatternOnEnemy>().StartOrNot = true;
        }
    }
}