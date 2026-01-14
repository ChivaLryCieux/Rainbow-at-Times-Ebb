using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using TMPro; 

public class PauseMenuMaster : MonoBehaviour
{
    [Header("--- 核心 UI ---")]
    public GameObject pauseMenuRoot; 
    
    [Header("--- 设置功能按钮 ---")]
    public Button btnResume; 
    public Button btnQuit;   

    [Header("--- 顶部导航栏 ---")]
    public TopTabButton[] topTabs; // 现在应该只剩 4 个元素了

    [Header("--- 内容面板 ---")]
    public GameObject settingsPanel;   
    public GameObject checkpointPanel; 
    public GameObject wikiSharedPanel; 

    [Header("--- 存档功能 ---")]
    public Button btnPrev;      
    public Button btnNext;      
    public Button btnReload;    
    public Text checkpointInfoText; 

    [Header("--- Wiki 左侧列表 ---")]
    public Transform leftListContent;    
    public GameObject titleButtonPrefab; 
    
    [Header("--- Wiki 右侧详情 ---")]
    public TextMeshProUGUI rightDetailBody; 
    public Image rightDetailImage;           
    public GameObject rightImageContainer;   

    private int currentIndex = 1; 
    private bool isPaused = false;

    void Start()
    {
        pauseMenuRoot.SetActive(false);
        if (btnResume != null) btnResume.onClick.AddListener(TogglePause);
        if (btnQuit != null) btnQuit.onClick.AddListener(() => {
            Time.timeScale = 1f; 
            SceneManager.LoadScene("IntroMenu"); 
        });

        if (btnPrev != null) btnPrev.onClick.AddListener(() => {
            if (GameManager.Instance != null) {
                GameManager.Instance.LoadCheckpoint(GameManager.Instance.CurrentIndex - 1);
                RefreshCheckpointUI(); 
            }
        });

        if (btnNext != null) btnNext.onClick.AddListener(() => {
            if (GameManager.Instance != null) {
                GameManager.Instance.LoadCheckpoint(GameManager.Instance.CurrentIndex + 1);
                RefreshCheckpointUI();
            }
        });

        if (btnReload != null) btnReload.onClick.AddListener(() => {
            if (GameManager.Instance != null) {
                GameManager.Instance.LoadCheckpoint(GameManager.Instance.CurrentIndex);
                TogglePause(); 
            }
        });
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

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
        if (isPaused) UpdateUI(); 
    }
    
    public void OnTabClicked(int index) { SwitchTab(index); }

    private void SwitchTab(int targetIndex)
    {
        currentIndex = targetIndex;
        // 1. 修改循环逻辑：上限从 4 变成 3
        if (currentIndex > 3) currentIndex = 0; 
        if (currentIndex < 0) currentIndex = 3;
        UpdateUI();
    }

    void UpdateUI()
    {
        // 更新顶部按钮状态
        if (topTabs != null)
        {
            for (int i = 0; i < topTabs.Length; i++)
            {
                if (topTabs[i] != null)
                    topTabs[i].SetSelected(i == currentIndex);
            }
        }

        if (settingsPanel != null) settingsPanel.SetActive(currentIndex == 0);
        if (checkpointPanel != null) checkpointPanel.SetActive(currentIndex == 1);
        
        // 2. 修改 Wiki 模式判断：索引范围现在是 2 到 3
        bool isWikiMode = (currentIndex >= 2 && currentIndex <= 3);
        if (wikiSharedPanel != null) wikiSharedPanel.SetActive(isWikiMode);

        if (currentIndex == 1) RefreshCheckpointUI();
        else if (isWikiMode)
        {
            if (currentIndex == 2) RefreshWikiList(WikiEntryData.Category.Research);
            // 3. 移除 Archive，并将 Event 的索引从 4 改为 3
            else if (currentIndex == 3) RefreshWikiList(WikiEntryData.Category.Event);
        }
    }

    void RefreshCheckpointUI()
    {
        if (GameManager.Instance == null) return;
        int current = GameManager.Instance.CurrentIndex;
        int max = GameManager.Instance.MaxIndex;
        if (btnPrev != null) btnPrev.interactable = (current > 0);
        if (btnNext != null) btnNext.interactable = (current < max);
        if (checkpointInfoText != null)
            checkpointInfoText.text = (current == 0) ? "Location: Start Point" : $"Checkpoint: {current} / {max}";
    }

    void RefreshWikiList(WikiEntryData.Category category)
    {
        foreach (Transform child in leftListContent) Destroy(child.gameObject);
        if (WikiManager.Instance == null) return;
        var entries = WikiManager.Instance.GetUnlockedEntriesByCategory(category);

        if (entries.Count == 0)
        {
            ClearRightPanel();
            return;
        }

        foreach (var entry in entries)
        {
            GameObject btnObj = Instantiate(titleButtonPrefab, leftListContent);
            WikiTitleButton btnScript = btnObj.GetComponent<WikiTitleButton>();
            if (btnScript != null) btnScript.Setup(entry, OnWikiTitleClicked);
        }
        OnWikiTitleClicked(entries[0]);
    }

    void OnWikiTitleClicked(WikiEntryData data)
    {
        if (rightDetailBody != null) rightDetailBody.text = data.description;
        if (rightImageContainer != null && rightDetailImage != null)
        {
            if (data.entryImage != null)
            {
                rightImageContainer.SetActive(true);
                rightDetailImage.sprite = data.entryImage;
                rightDetailImage.preserveAspect = true;
            }
            else rightImageContainer.SetActive(false); 
        }
    }

    void ClearRightPanel()
    {
        if (rightDetailBody != null) rightDetailBody.text = "暂无数据";
        if (rightImageContainer != null) rightImageContainer.SetActive(false);
    }
}