using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.SceneManagement;

public class FinalMenuController : MonoBehaviour
{
    public static FinalMenuController Instance;

    [Header("--- UI 组件 ---")]
    public GameObject finalCanvasRoot;    
    public GameObject videoPanel;         
    public CanvasGroup menuCanvasGroup;   
    
    [Header("--- 视频组件 ---")]
    public VideoPlayer videoPlayer;       
    // 注意：这里不再需要原本那个单一的 endingVideoClip 变量了，
    // 因为视频现在由触发器传过来。

    [Header("--- 按钮 ---")]
    public Button btnRestart;
    public Button btnQuit;

    [Header("--- 引用 ---")]
    public MonoBehaviour playerScript;       
    public CameraFollow cameraFollowScript;  
    public GameObject hudCanvas;             

    private bool isEnding = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (finalCanvasRoot != null) finalCanvasRoot.SetActive(false);
        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 0f;
            menuCanvasGroup.interactable = false;
            menuCanvasGroup.blocksRaycasts = false;
        }

        if (btnRestart != null) btnRestart.onClick.AddListener(OnRestartClicked);
        if (btnQuit != null) btnQuit.onClick.AddListener(OnQuitClicked);
    }

    // 【关键修改】增加参数：targetClip
    // 哪个触发器调用我，就把它的视频传给我
    public void StartEndingSequence(VideoClip targetClip)
    {
        if (isEnding) return;
        
        // 如果触发器忘了拖视频，报个错防止卡死
        if (targetClip == null)
        {
            Debug.LogError("触发器未指定视频！无法播放结局。");
            return;
        }

        isEnding = true;

        // 1. 禁用控制
        if (playerScript != null) playerScript.enabled = false;
        if (cameraFollowScript != null) cameraFollowScript.enabled = false;
        if (hudCanvas != null) hudCanvas.SetActive(false);

        // 2. 锁住暂停菜单
        if (PauseMenuMaster.Instance != null) 
        {
            PauseMenuMaster.Instance.SetGameStarted(false); 
        }

        // 3. 激活 UI 并开始播放指定的视频
        if (finalCanvasRoot != null) finalCanvasRoot.SetActive(true);
        StartCoroutine(PlayVideoRoutine(targetClip));
    }

    // 【修改】协程接收参数
    IEnumerator PlayVideoRoutine(VideoClip clipToPlay)
    {
        // --- 阶段 A: 播放视频 ---
        if (videoPlayer != null)
        {
            videoPlayer.clip = clipToPlay; // 【关键】播放传进来的视频
            videoPlayer.Prepare();

            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }

            videoPlayer.Play();
            Debug.Log($"开始播放结局: {clipToPlay.name}");

            float timer = 0f;
            while (true)
            {
                timer += Time.unscaledDeltaTime;

                // 播放结束判定
                if (timer > 1f && (ulong)videoPlayer.frame >= videoPlayer.frameCount - 5)
                {
                    break;
                }

                // 跳过判定
                if (Input.anyKeyDown && timer > 0.5f) 
                {
                    videoPlayer.Stop();
                    break;
                }

                yield return null;
            }
        }

        // --- 阶段 B: 显示菜单 ---
        if (videoPanel != null) videoPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 开启点击检测 (解决刚才点不动的问题)
        if (menuCanvasGroup != null) menuCanvasGroup.blocksRaycasts = true;

        float fadeTimer = 0f;
        while (fadeTimer < 1f)
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (menuCanvasGroup != null) menuCanvasGroup.alpha = fadeTimer;
            yield return null;
        }

        if (menuCanvasGroup != null)
        {
            menuCanvasGroup.alpha = 1f;
            menuCanvasGroup.interactable = true;
        }
    }

    // --- 按钮逻辑 ---
    void OnRestartClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReloadScene();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnQuitClicked()
    {
        if (GameManager.Instance != null) GameManager.Instance.QuitGame();
        else Application.Quit();
    }
}