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

    [Header("--- Wiki 左侧列表 ---")]
    public Transform leftListContent;    
    public GameObject titleButtonPrefab; 
    
    [Header("--- Wiki 右侧详情 (无标题) ---")]
    public TextMeshProUGUI rightDetailBody; 
    public Image rightDetailImage;           
    public GameObject rightImageContainer;   

    private int currentIndex = 1; 
    private bool isPaused = false;

    void Start()
    {
        pauseMenuRoot.SetActive(false);
        
        if (btnResume != null) btnResume.onClick.AddListener(TogglePause);
        
        if (btnQuit != null)
        {
            btnQuit.onClick.AddListener(() => 
            {
                Time.timeScale = 1f; 
                SceneManager.LoadScene("IntroMenu"); 
            });
        }

        // 绑定存档按钮
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
        if (currentIndex > 4) currentIndex = 0;
        if (currentIndex < 0) currentIndex = 4;
        UpdateUI();
    }

    void UpdateUI()
    {
        for (int i = 0; i < topButtonImages.Length; i++)
        {
            if (topButtonImages[i] != null)
                topButtonImages[i].color = (i == currentIndex) ? activeColor : inactiveColor;
        }

        if (settingsPanel != null) settingsPanel.SetActive(currentIndex == 0);
        if (checkpointPanel != null) checkpointPanel.SetActive(currentIndex == 1);
        
        bool isWikiMode = (currentIndex >= 2 && currentIndex <= 4);
        if (wikiSharedPanel != null) wikiSharedPanel.SetActive(isWikiMode);

        if (currentIndex == 1) RefreshCheckpointUI();
        else if (isWikiMode)
        {
            if (currentIndex == 2) RefreshWikiList(WikiEntryData.Category.Research);
            else if (currentIndex == 3) RefreshWikiList(WikiEntryData.Category.Archive);
            else if (currentIndex == 4) RefreshWikiList(WikiEntryData.Category.Event);
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
            if (btnScript != null)
            {
                btnScript.Setup(entry, OnWikiTitleClicked);
            }
        }
        OnWikiTitleClicked(entries[0]);
    }

    // --- 点击处理：更新图片和正文 ---
    void OnWikiTitleClicked(WikiEntryData data)
    {
        // 只更新正文
        if (rightDetailBody != null) rightDetailBody.text = data.description;

        // 更新图片
        if (rightImageContainer != null && rightDetailImage != null)
        {
            if (data.entryImage != null)
            {
                rightImageContainer.SetActive(true);
                rightDetailImage.sprite = data.entryImage;
                rightDetailImage.preserveAspect = true;
            }
            else
            {
                rightImageContainer.SetActive(false); 
            }
        }
    }

    void ClearRightPanel()
    {
        // 移除标题清空逻辑
        if (rightDetailBody != null) rightDetailBody.text = "暂无数据";
        if (rightImageContainer != null) rightImageContainer.SetActive(false);
    }
}