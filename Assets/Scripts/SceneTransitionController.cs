using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionController : MonoBehaviour
{
    // 正在加载场景名称
    private string _nextScene = "";
    private bool _reload;

    private void Awake()
    {
        Animator animator = gameObject.GetComponentInChildren<Animator>();
        animator.Play("FadeIn");
    }

    // 从淡出动画结束时触发
    public void FadeOutFinished()
    {
        // 下一个场景的加载
        SceneManager.LoadScene(_nextScene);
    }

    public void SetTransition(string nextScene)
    {
        _reload = true;
        _nextScene = nextScene;
    }
}
