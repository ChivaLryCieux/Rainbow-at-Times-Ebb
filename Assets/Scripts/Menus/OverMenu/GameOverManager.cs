using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    // 在场景加载时立刻恢复时间，否则菜单动画可能不播放，或者下一局游戏卡住
    private void Start()
    {
        Time.timeScale = 1f; 
        
        // 确保鼠标可见，方便点按钮
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnTryAgainClicked()
    {
        // 再次确保时间是流动的
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainScene");
    }

    public void OnQuitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("IntroMenu");
    }
}