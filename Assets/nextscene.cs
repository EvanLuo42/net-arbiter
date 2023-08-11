using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    private Animator animator;
    
    public string sceneName;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetTransition()
    {
        Debug.Log("ABC");
        animator.SetTrigger("ChangeToNextScene");
    }
    // Update is called once per frame
    void ChangeScene()
    {
        SceneManager.LoadScene(sceneName);
    }
}
