using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    void OnAnimationDone()
    {
        SceneManager.LoadScene(2);
    }
}
