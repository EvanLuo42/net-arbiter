using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    // 正在加载场景名称
    private string _nextScene = "";
    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
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
        _animator.SetTrigger("FadeOut");
    }
}
