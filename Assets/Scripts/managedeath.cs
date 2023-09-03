using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managedeath : MonoBehaviour
{
    public static managedeath Instance;
    public Vector2 lastPosition;
    //string sceneName;
    // Start is called before the first frame update
    void Awake()
    {
        //sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        //Check();
        Instance = this;
        lastPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
    }
    //private void Start()
    //{
    //    Check();
    //}


    //public void Check()
    //{
    //    if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != sceneName)
    //    {
    //        Destroy(this);
    //    }
    //}
    // Update is called once per frame
}
