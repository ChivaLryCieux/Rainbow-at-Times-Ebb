using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    public Button tryAgainButton;
    public Button quitButton;

    void Start()
    {
        // 绑定 Try Again 按钮到 GameManager 的 ReloadScene
        tryAgainButton.onClick.AddListener(() =>
        {
            GameManager.Instance.ReloadScene();
        });

        // 绑定 Quit
        quitButton.onClick.AddListener(() =>
        {
            GameManager.Instance.QuitGame();
        });
    }

    public void ShowGameOver()
    {
        gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}