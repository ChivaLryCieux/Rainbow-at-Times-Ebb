using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI 面板")]
    public GameObject pausePanel;

    [Header("核心按钮")]
    public Button btnResume;    // 新增：继续游戏（仅关闭菜单）
    public Button btnQuit;      // 退出游戏
    public Button btnInventory; // 背包

    [Header("存档点导航按钮")]
    public Button btnPrev;      // 上一个 (<)
    public Button btnReload;    // 重玩当前检查点 (⟳)
    public Button btnNext;      // 下一个 (>)

    [Header("信息显示")]
    public Text checkpointInfoText; // 显示 "Checkpoint: 2 / 5"

    private bool isPaused = false;

    private void Start()
    {
        pausePanel.SetActive(false);

        // --- 1. 核心功能绑定 ---
        // 继续游戏：只恢复时间，不传送
        btnResume.onClick.AddListener(ResumeGame);

        // 退出游戏
        btnQuit.onClick.AddListener(() => GameManager.Instance.QuitGame());

        // 背包（占位）
        btnInventory.onClick.AddListener(() => Debug.Log("打开背包"));

        // --- 2. 存档导航绑定 ---
        // 上一个：加载 index - 1
        btnPrev.onClick.AddListener(() =>
        {
            int target = GameManager.Instance.CurrentIndex - 1;
            GameManager.Instance.LoadCheckpoint(target);
            RefreshUIState();
            // 注意：这里我们不调用 ResumeGame()，允许玩家连续点击切关
            // 如果你想切关后立刻关闭菜单，可以在这里加 ResumeGame();
        });

        // 下一个：加载 index + 1
        btnNext.onClick.AddListener(() =>
        {
            int target = GameManager.Instance.CurrentIndex + 1;
            GameManager.Instance.LoadCheckpoint(target);
            RefreshUIState();
        });

        // 重载当前：传送回当前存档点位置，并关闭菜单继续游玩
        btnReload.onClick.AddListener(() =>
        {
            GameManager.Instance.LoadCheckpoint(GameManager.Instance.CurrentIndex);
            ResumeGame();
        });
    }

    private void Update()
    {
        // 监听 Esc 键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 暂停时间

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RefreshUIState();
    }

    private void ResumeGame()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // 恢复时间

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 刷新按钮状态（置灰逻辑）
    private void RefreshUIState()
    {
        int current = GameManager.Instance.CurrentIndex;
        int max = GameManager.Instance.MaxIndex;

        btnPrev.interactable = (current > 0);
        btnNext.interactable = (current < max);

        if (checkpointInfoText != null)
        {
            // 如果是 0，显示 "Start"，否则显示数字
            if (current == 0)
            {
                checkpointInfoText.text = "Location: Start Point";
            }
            else
            {
                checkpointInfoText.text = $"Checkpoint: {current} / {max}";
            }
        }
    }
}