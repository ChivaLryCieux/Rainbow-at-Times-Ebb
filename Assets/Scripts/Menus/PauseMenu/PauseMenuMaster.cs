using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.Video; 

public class PauseMenuMaster : MonoBehaviour
{
    [Header("--- 核心 UI ---")]
    public GameObject pauseMenuRoot; 
    public TopTabButton[] topTabs; 

    // 省略了其他通用面板变量 (Settings, Checkpoint 等保持不变) ...
    [Header("--- 面板引用 ---")]
    public GameObject settingsPanel;   
    public GameObject checkpointPanel; 
    public GameObject wikiSharedPanel; 
    
    // 省略存档按钮变量 ...
    [Header("--- 存档/通用按钮 ---")]
    public Button btnResume; 
    public Button btnQuit;
    public Button btnPrev;      
    public Button btnNext;      
    public Button btnReload;    
    public Text checkpointInfoText; 

    [Header("--- Wiki 左侧列表 ---")]
    public Transform leftListContent;    
    public GameObject titleButtonPrefab; 

    // ==========================================
    //  新增/修改的 Wiki UI 引用
    // ==========================================
    
    [Header("--- Wiki: 研究 (Research) 布局 ---")]
    public GameObject researchLayoutRoot; // 包含两个并排图片的父物体
    public Image researchLeftImage;       // 左图
    public Image researchRightImage;      // 右图

    [Header("--- Wiki: 事件 (Event) 布局 ---")]
    public GameObject eventLayoutRoot;    // 包含单图和播放按钮的父物体
    public Image eventMainImage;          // 事件主图
    public Button eventPlayButton;        // 播放按钮

    [Header("--- Wiki: 全屏视频弹窗 ---")]
    public GameObject videoOverlayRoot;   // 黑色半透明背景层
    public VideoPlayer fullscreenVideoPlayer; // 视频播放器组件
    public Button closeVideoButton;       // 关闭视频的按钮

    // ==========================================

    private int currentIndex = 1; 
    private bool isPaused = false;
    
    // 缓存当前选中的视频，供按钮调用
    private VideoClip currentVideoClip; 

    void Start()
    {
        pauseMenuRoot.SetActive(false);
        if (videoOverlayRoot != null) videoOverlayRoot.SetActive(false); // 确保视频层开始是关闭的

        // 绑定基础按钮事件
        if (btnResume != null) btnResume.onClick.AddListener(TogglePause);
        if (btnQuit != null) btnQuit.onClick.AddListener(() => {
            Time.timeScale = 1f; 
            SceneManager.LoadScene("IntroMenu"); 
        });

        // 绑定视频相关事件
        if (eventPlayButton != null) 
            eventPlayButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (closeVideoButton != null) 
            closeVideoButton.onClick.AddListener(CloseVideoOverlay);
        
        // 绑定存档按钮 (保持原样，省略代码以节省篇幅)
        SetupCheckpointButtons();
    }
    
    // 为了代码整洁，把Start里的一堆绑定挪出来了，你保留原来的即可
    void SetupCheckpointButtons() {
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
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            // 如果视频弹窗开着，按 ESC 优先关闭视频，而不是关闭整个菜单
            if (videoOverlayRoot != null && videoOverlayRoot.activeSelf) {
                CloseVideoOverlay();
            }
            else {
                TogglePause();
            }
        }

        if (isPaused && !videoOverlayRoot.activeSelf) // 视频播放时禁止切换 Tab
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchTab(currentIndex - 1);
            if (Input.GetKeyDown(KeyCode.E)) SwitchTab(currentIndex + 1);
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuRoot.SetActive(isPaused);
        
        // 确保打开菜单时视频层是关的
        if (videoOverlayRoot != null) videoOverlayRoot.SetActive(false);

        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (isPaused) UpdateUI(); 
    }
    
    public void OnTabClicked(int index) { SwitchTab(index); }

    private void SwitchTab(int targetIndex)
    {
        currentIndex = targetIndex;
        if (currentIndex > 3) currentIndex = 0; 
        if (currentIndex < 0) currentIndex = 3;
        UpdateUI();
    }

    void UpdateUI()
    {
        // 顶部导航状态
        if (topTabs != null) {
            for (int i = 0; i < topTabs.Length; i++) {
                if (topTabs[i] != null) topTabs[i].SetSelected(i == currentIndex);
            }
        }

        if (settingsPanel != null) settingsPanel.SetActive(currentIndex == 0);
        if (checkpointPanel != null) checkpointPanel.SetActive(currentIndex == 1);
        
        bool isWikiMode = (currentIndex >= 2 && currentIndex <= 3);
        if (wikiSharedPanel != null) wikiSharedPanel.SetActive(isWikiMode);

        if (currentIndex == 1) RefreshCheckpointUI();
        else if (isWikiMode)
        {
            // 2 = Research, 3 = Event
            if (currentIndex == 2) RefreshWikiList(WikiEntryData.Category.Research);
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
            // 清空右侧显示
            if (researchLayoutRoot) researchLayoutRoot.SetActive(false);
            if (eventLayoutRoot) eventLayoutRoot.SetActive(false);
            return;
        }

        foreach (var entry in entries)
        {
            GameObject btnObj = Instantiate(titleButtonPrefab, leftListContent);
            WikiTitleButton btnScript = btnObj.GetComponent<WikiTitleButton>();
            if (btnScript != null) btnScript.Setup(entry, OnWikiTitleClicked);
        }
        
        // 默认选中第一个
        OnWikiTitleClicked(entries[0]);
    }

    // ==========================================
    //  核心逻辑修改：根据 Category 切换布局
    // ==========================================
    void OnWikiTitleClicked(WikiEntryData data)
    {
        // 1. 根据类别显隐不同的布局容器
        bool isResearch = (data.category == WikiEntryData.Category.Research);
        
        if (researchLayoutRoot != null) researchLayoutRoot.SetActive(isResearch);
        if (eventLayoutRoot != null) eventLayoutRoot.SetActive(!isResearch);

        if (isResearch)
        {
            // --- Research 模式：双图并排，无文字 ---
            if (researchLeftImage != null)
            {
                researchLeftImage.gameObject.SetActive(data.primaryImage != null);
                researchLeftImage.sprite = data.primaryImage;
                // 如果需要保持比例： researchLeftImage.preserveAspect = true;
            }
            if (researchRightImage != null)
            {
                researchRightImage.gameObject.SetActive(data.secondaryImage != null);
                researchRightImage.sprite = data.secondaryImage;
            }
        }
        else
        {
            // --- Event 模式：单图 + 播放按钮 ---
            if (eventMainImage != null)
            {
                eventMainImage.gameObject.SetActive(data.primaryImage != null);
                eventMainImage.sprite = data.primaryImage;
            }

            // 检查是否有视频
            if (data.HasVideo())
            {
                currentVideoClip = data.wikiVideo; // 缓存视频引用
                if (eventPlayButton != null) eventPlayButton.gameObject.SetActive(true);
            }
            else
            {
                currentVideoClip = null;
                if (eventPlayButton != null) eventPlayButton.gameObject.SetActive(false);
            }
        }
    }

    // 点击播放按钮时触发
    void OnPlayButtonClicked()
    {
        if (currentVideoClip != null && fullscreenVideoPlayer != null)
        {
            videoOverlayRoot.SetActive(true); // 打开遮罩层
            fullscreenVideoPlayer.clip = currentVideoClip;
            fullscreenVideoPlayer.Play();
        }
    }

    // 关闭视频遮罩
    void CloseVideoOverlay()
    {
        if (fullscreenVideoPlayer != null)
        {
            fullscreenVideoPlayer.Stop();
            fullscreenVideoPlayer.clip = null; // 清理引用
        }
        if (videoOverlayRoot != null)
        {
            videoOverlayRoot.SetActive(false);
        }
    }
}