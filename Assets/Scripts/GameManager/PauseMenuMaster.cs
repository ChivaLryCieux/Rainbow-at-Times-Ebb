using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 

public class PauseMenuMaster : MonoBehaviour
{
    [Header("--- 核心 UI ---")]
    public GameObject pauseMenuRoot; 
    
    [Header("--- 设置功能按钮 ---")]
    public Button btnResume; // 继续游戏
    public Button btnQuit;   // 退出游戏

    [Header("--- 顶部导航栏 ---")]
    public Image[] topButtonImages; 
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("--- 内容面板 ---")]
    public GameObject settingsPanel;   
    public GameObject checkpointPanel; 
    public GameObject wikiSharedPanel; 

    [Header("--- 存档功能 ---")]
    public Button btnPrev;      
    public Button btnNext;      
    public Button btnReload;    
    public Text checkpointInfoText; 

    [Header("--- Wiki 数据生成 ---")]
    public Transform wikiContentParent;
    public GameObject wikiEntryPrefab;

    private int currentIndex = 1; 
    private bool isPaused = false;

    void Start()
    {
        pauseMenuRoot.SetActive(false);
        
        // 1. 继续游戏：直接调用 TogglePause 即可，因为它就是负责开关界面的
        if (btnResume != null)
        {
            btnResume.onClick.AddListener(TogglePause);
        }

        // 2. 退出游戏：必须先恢复时间，再跳转场景
        if (btnQuit != null)
        {
            btnQuit.onClick.AddListener(() => 
            {
                Time.timeScale = 1f; // ⚠️ 极其重要！如果不写这句，回到主菜单游戏依然是暂停的
                SceneManager.LoadScene("IntroMenu"); // 确保你的主菜单场景名字真的是 "IntroMenu"
            });
        }

        // --- 存档按钮绑定 ---
        btnPrev.onClick.AddListener(() => {
            int target = GameManager.Instance.CurrentIndex - 1;
            GameManager.Instance.LoadCheckpoint(target);
            RefreshCheckpointUI(); 
        });

        btnNext.onClick.AddListener(() => {
            int target = GameManager.Instance.CurrentIndex + 1;
            GameManager.Instance.LoadCheckpoint(target);
            RefreshCheckpointUI();
        });

        btnReload.onClick.AddListener(() => {
            GameManager.Instance.LoadCheckpoint(GameManager.Instance.CurrentIndex);
            TogglePause(); 
        });
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchTab(currentIndex - 1);
            if (Input.GetKeyDown(KeyCode.E)) SwitchTab(currentIndex + 1);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuRoot.SetActive(isPaused);
        
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (isPaused)
        {
            UpdateUI(); 
        }
    }
    
    public void OnTabClicked(int index) { SwitchTab(index); }

    private void SwitchTab(int targetIndex)
    {
        currentIndex = targetIndex;
        if (currentIndex > 4) currentIndex = 0;
        if (currentIndex < 0) currentIndex = 4;
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < topButtonImages.Length; i++)
            topButtonImages[i].color = (i == currentIndex) ? activeColor : inactiveColor;

        settingsPanel.SetActive(currentIndex == 0);
        checkpointPanel.SetActive(currentIndex == 1);
        bool isWikiMode = (currentIndex >= 2 && currentIndex <= 4);
        wikiSharedPanel.SetActive(isWikiMode);

        if (currentIndex == 1) RefreshCheckpointUI();
        else if (isWikiMode)
        {
            if (currentIndex == 2) RefreshWiki(WikiEntryData.Category.Research);
            else if (currentIndex == 3) RefreshWiki(WikiEntryData.Category.Archive);
            else if (currentIndex == 4) RefreshWiki(WikiEntryData.Category.Event);
        }
    }

    void RefreshCheckpointUI()
    {
        if (GameManager.Instance == null) return;
        int current = GameManager.Instance.CurrentIndex;
        int max = GameManager.Instance.MaxIndex;
        btnPrev.interactable = (current > 0);
        btnNext.interactable = (current < max);
        if (checkpointInfoText != null)
            checkpointInfoText.text = (current == 0) ? "Location: Start Point" : $"Checkpoint: {current} / {max}";
    }

    void RefreshWiki(WikiEntryData.Category category)
    {
        foreach (Transform child in wikiContentParent) Destroy(child.gameObject);
        var entries = WikiManager.Instance.GetUnlockedEntriesByCategory(category);
        foreach (var entry in entries)
        {
            GameObject obj = Instantiate(wikiEntryPrefab, wikiContentParent);
            obj.GetComponent<WikiUIItem>().Setup(entry.title, entry.description);
        }
    }
}