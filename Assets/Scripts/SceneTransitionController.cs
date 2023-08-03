using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    // 正在加载场景名称
    private string _nextScene = "";
    private Animator animator;

    private void Start()
    {
        animator = gameObject.GetComponentInChildren<Animator>();
    }

    // 从淡出动画结束时触发
    private void FadeOutFinished()
    {
        // 下一个场景的加载
        SceneManager.LoadScene(_nextScene);
    }

    public void SetTransition(string nextScene)
    {
        _nextScene = nextScene;
        animator.SetTrigger("FadeOut");
    }
}
