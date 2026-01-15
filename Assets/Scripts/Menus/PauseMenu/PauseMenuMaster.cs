using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 必须引用，用于重载场景
using TMPro; 
using UnityEngine.Video; 

public class PauseMenuMaster : MonoBehaviour
{
    public static PauseMenuMaster Instance;

    [Header("--- 核心 UI ---")]
    public GameObject pauseMenuRoot; 
    public TopTabButton[] topTabs; 

    // =========== HUD 引用 (暂停时隐藏) ===========
    [Header("--- HUD (暂停时隐藏) ---")]
    public GameObject pickupSubtitleObj;    // 请拖入 PickupSubtitle
    public GameObject voiceoverSubtitleObj; // 请拖入 VoiceoverSubtitle
    // ==========================================
    
    [Header("--- 面板引用 ---")]
    public GameObject settingsPanel;   
    public GameObject checkpointPanel; 
    public GameObject wikiSharedPanel; 
    
    [Header("--- 存档/通用按钮 ---")]
    public Button btnResume; 
    public Button btnQuit;
    public Button btnPrev;      
    public Button btnNext;      
    public Button btnReload;    // 这里将作为“重新开始/回到标题”按钮
    public Text checkpointInfoText; 

    [Header("--- Wiki 左侧列表 ---")]
    public Transform leftListContent;    
    public GameObject titleButtonPrefab; 
    
    [Header("--- Wiki: 研究 (Research) 布局 ---")]
    public GameObject researchLayoutRoot; 
    public Image researchLeftImage;       
    public Image researchRightImage;      

    [Header("--- Wiki: 事件 (Event) 布局 ---")]
    public GameObject eventLayoutRoot;    
    public Image eventMainImage;          
    public Button eventPlayButton;        

    [Header("--- Wiki: 全屏视频弹窗 ---")]
    public GameObject videoOverlayRoot;   
    public VideoPlayer fullscreenVideoPlayer; 
    public Button closeVideoButton;       

    private int currentIndex = 1; 
    private bool isPaused = false;
    
    // 【新增】用来标记是否允许暂停（默认为false，等BeginGameController通知变为true）
    private bool canPause = false; 

    private VideoClip currentVideoClip; 

    void Awake() 
    {
        if (Instance == null) Instance = this;
        
        // 游戏刚运行时不允许暂停（防止在开场动画时按ESC）
        canPause = false;
    }

    void Start()
    {
        pauseMenuRoot.SetActive(false);
        if (videoOverlayRoot != null) videoOverlayRoot.SetActive(false); 

        // --- 按钮绑定 ---
        
        // 1. 继续游戏
        if (btnResume != null) btnResume.onClick.AddListener(TogglePause);
        
        // 2. 退出游戏 (完全退出程序)
        if (btnQuit != null) btnQuit.onClick.AddListener(() => {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });

        // 3. 重新开始 / 重载 (实现“一切还原并回到BeginCanvas”)
        if (btnReload != null) btnReload.onClick.AddListener(() => {
            // 恢复时间流速，否则重载后的场景一开始就是暂停的
            Time.timeScale = 1f; 
            // 重载当前活动的场景 -> 这会重置所有脚本、变量，BeginCanvas 也会重新出现
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        // 4. 视频相关
        if (eventPlayButton != null) 
            eventPlayButton.onClick.AddListener(OnPlayButtonClicked);
        
        if (closeVideoButton != null) 
            closeVideoButton.onClick.AddListener(CloseVideoOverlay);
        
        // 5. 存档相关按钮
        SetupCheckpointButtons();
    }
    
    // --- 外部调用：设置游戏是否正式开始 ---
    // 由 BeginGameController 在动画播放完毕后调用 SetGameStarted(true)
    public void SetGameStarted(bool state)
    {
        canPause = state;
    }

    // --- 外部调用：强制打开菜单并播放视频 ---
    public void OpenAndPlayVideo(string entryID)
    {
        // 1. 强制暂停并打开菜单
        if (!isPaused) TogglePause();

        // 2. 找到数据
        WikiEntryData data = WikiManager.Instance.GetEntryByID(entryID);
        if (data == null) return;

        // 3. 切换到 Event 页面 (假设 Index 3 是 Event)
        SwitchTab(3); 

        // 4. 刷新并选中该条目
        OnWikiTitleClicked(data);

        // 5. 自动播放
        if (data.HasVideo())
        {
            currentVideoClip = data.wikiVideo;
            OnPlayButtonClicked(); 
        }
    }

    // --- 核心暂停逻辑 ---
    public void TogglePause()
    {
        // 如果不允许暂停（比如还在开场动画），直接无视
        if (!canPause && !isPaused) return;

        isPaused = !isPaused;
        
        // 1. 控制菜单显隐
        pauseMenuRoot.SetActive(isPaused);
        
        // 2. 控制 HUD 字幕显隐 (暂停时隐藏，游戏时显示)
        if (pickupSubtitleObj != null) pickupSubtitleObj.SetActive(!isPaused);
        if (voiceoverSubtitleObj != null) voiceoverSubtitleObj.SetActive(!isPaused);

        // 3. 确保视频层关闭
        if (videoOverlayRoot != null) videoOverlayRoot.SetActive(false);

        // 4. 游戏状态控制
        Time.timeScale = isPaused ? 0f : 1f;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;

        if (isPaused) UpdateUI(); 
    }

    void Update()
    {
        // 如果不允许暂停，且当前没处于暂停状态，就不检测 ESC
        if (!canPause && !isPaused) return;

        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (videoOverlayRoot != null && videoOverlayRoot.activeSelf) 
                CloseVideoOverlay();
            else 
                TogglePause();
        }

        if (isPaused && !videoOverlayRoot.activeSelf) 
        {
            if (Input.GetKeyDown(KeyCode.Q)) SwitchTab(currentIndex - 1);
            if (Input.GetKeyDown(KeyCode.E)) SwitchTab(currentIndex + 1);
        }
    }

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
        // btnReload 的逻辑已经移到 Start 里面单独处理了，为了实现重载场景
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
        
        OnWikiTitleClicked(entries[0]);
    }

    void OnWikiTitleClicked(WikiEntryData data)
    {
        bool isResearch = (data.category == WikiEntryData.Category.Research);
        
        if (researchLayoutRoot != null) researchLayoutRoot.SetActive(isResearch);
        if (eventLayoutRoot != null) eventLayoutRoot.SetActive(!isResearch);

        if (isResearch)
        {
            if (researchLeftImage != null)
            {
                researchLeftImage.gameObject.SetActive(data.primaryImage != null);
                researchLeftImage.sprite = data.primaryImage;
            }
            if (researchRightImage != null)
            {
                researchRightImage.gameObject.SetActive(data.secondaryImage != null);
                researchRightImage.sprite = data.secondaryImage;
            }
        }
        else
        {
            if (eventMainImage != null)
            {
                eventMainImage.gameObject.SetActive(data.primaryImage != null);
                eventMainImage.sprite = data.primaryImage;
            }

            if (data.HasVideo())
            {
                currentVideoClip = data.wikiVideo; 
                if (eventPlayButton != null) eventPlayButton.gameObject.SetActive(true);
            }
            else
            {
                currentVideoClip = null;
                if (eventPlayButton != null) eventPlayButton.gameObject.SetActive(false);
            }
        }
    }

    void OnPlayButtonClicked()
    {
        if (currentVideoClip != null && fullscreenVideoPlayer != null)
        {
            videoOverlayRoot.SetActive(true); 
            fullscreenVideoPlayer.clip = currentVideoClip;
            fullscreenVideoPlayer.Play();
        }
    }

    void CloseVideoOverlay()
    {
        if (fullscreenVideoPlayer != null)
        {
            fullscreenVideoPlayer.Stop();
            fullscreenVideoPlayer.clip = null; 
        }
        if (videoOverlayRoot != null)
        {
            videoOverlayRoot.SetActive(false);
        }
    }
}