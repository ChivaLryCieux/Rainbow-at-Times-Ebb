using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class OverMenuController : MonoBehaviour
{
    public static OverMenuController Instance;

    [Header("--- UI 设置 ---")]
    public GameObject overMenuRoot;      
    public CanvasGroup uiCanvasGroup;    
    public float fadeDuration = 1.5f;    // UI 淡入时间

    [Header("--- 摄像机设置 ---")]
    public Transform mainCamera;         
    public CameraFollow cameraFollowScript; 

    [Header("--- 按钮 ---")]
    public Button btnLoadCheckpoint;
    public Button btnQuit;

    private bool isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        if (overMenuRoot != null) overMenuRoot.SetActive(false);
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }

        if (btnLoadCheckpoint != null)
            btnLoadCheckpoint.onClick.AddListener(OnLoadCheckpointClicked);
        
        if (btnQuit != null)
            btnQuit.onClick.AddListener(OnQuitClicked);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 1. 直接禁用跟随脚本
        // 这一步之后，摄像机的位置就被“定”在当前这一帧了，不会再移动
        if (cameraFollowScript != null)
        {
            cameraFollowScript.enabled = false;
        }

        // 2. 激活 UI
        if (overMenuRoot != null) overMenuRoot.SetActive(true);

        // 3. 开始流程：淡入UI + 镜头注视
        StartCoroutine(GameOverSequence());

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator GameOverSequence()
    {
        float timer = 0f;
        
        // 记录一下摄像机开始时的旋转，用于平滑过渡
        Quaternion startRot = mainCamera.rotation;

        // 我们假设要注视的目标是主角 (cameraFollowScript.target)
        Transform targetToLookAt = null;
        if (cameraFollowScript != null) targetToLookAt = cameraFollowScript.target;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration; // 归一化时间 0~1

            // --- A. 镜头逻辑：原地不动，但旋转注视 ---
            if (mainCamera != null && targetToLookAt != null)
            {
                // 计算“看向玩家”的旋转角度
                Vector3 directionToPlayer = targetToLookAt.position - mainCamera.position;
                if (directionToPlayer != Vector3.zero)
                {
                    Quaternion lookRot = Quaternion.LookRotation(directionToPlayer);
                    // 平滑转动视角去盯着玩家 (如果不想转动，把下面这行注释掉即可)
                    mainCamera.rotation = Quaternion.Slerp(startRot, lookRot, t * 2f); // *2 加快一点转头速度
                }
            }
            // 注意：这里没有修改 mainCamera.position，所以位置是绝对静止的

            // --- B. UI 淡入 ---
            if (uiCanvasGroup != null)
            {
                uiCanvasGroup.alpha = t;
            }

            yield return null;
        }

        // 确保 UI 最终完全可见
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 1f;
            uiCanvasGroup.interactable = true;
            uiCanvasGroup.blocksRaycasts = true;
        }
    }

    // --- 按钮逻辑 ---
    void OnLoadCheckpointClicked()
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}