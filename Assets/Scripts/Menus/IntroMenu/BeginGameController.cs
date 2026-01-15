using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BeginGameController : MonoBehaviour
{
    [Header("--- UI 设置 ---")]
    public CanvasGroup uiCanvasGroup; 
    public float fadeDuration = 1.0f; 

    [Header("--- 摄像机动画 ---")]
    public Transform mainCamera;      
    public Transform introPoint;      
    public CameraFollow cameraFollowScript; 
    public float moveDuration = 2.0f; 

    [Header("--- 玩家控制 ---")]
    public MonoBehaviour playerScript; 
    public PauseMenuMaster pauseMaster; 
    
    // 【新增】用来控制主角的刚体
    public Rigidbody playerRigidbody; 

    private bool hasStarted = false;

    void Start()
    {
        // 自动查找
        if (pauseMaster == null) pauseMaster = FindObjectOfType<PauseMenuMaster>();
        
        // 【新增】如果没拖拽，尝试自动从 PlayerScript 上找 Rigidbody
        if (playerRigidbody == null && playerScript != null)
            playerRigidbody = playerScript.GetComponent<Rigidbody>();

        hasStarted = false;
        Time.timeScale = 1f; 

        // 1. 禁用玩家控制脚本
        if (playerScript != null) playerScript.enabled = false;
        
        // 2. 【核心修复】将刚体设为 Kinematic，钉死在原地，防止穿模掉落
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true; 
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (cameraFollowScript != null) cameraFollowScript.enabled = false;

        if (mainCamera != null && introPoint != null)
        {
            mainCamera.position = introPoint.position;
            mainCamera.rotation = introPoint.rotation;
        }
    }

    void Update()
    {
        if (!hasStarted && Input.anyKeyDown)
        {
            StartCoroutine(EnterGameSequence());
        }
    }

    IEnumerator EnterGameSequence()
    {
        hasStarted = true;

        // --- 阶段 1: UI 淡出 ---
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (uiCanvasGroup != null) uiCanvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeDuration);
            yield return null;
        }
        
        if (uiCanvasGroup != null)
        {
            uiCanvasGroup.alpha = 0f;
            uiCanvasGroup.interactable = false;
            uiCanvasGroup.blocksRaycasts = false;
        }

        // --- 阶段 2: 摄像机推拉 ---
        Vector3 finalPos = mainCamera.position;
        Quaternion finalRot = Quaternion.identity; 

        if (cameraFollowScript != null && cameraFollowScript.target != null)
        {
            Vector3 tPos = cameraFollowScript.target.position;
            Vector3 offset = cameraFollowScript.offset;
            finalPos = new Vector3(tPos.x, tPos.y + offset.y, tPos.z + offset.z);
            finalRot = Quaternion.identity; 
        }

        timer = 0f;
        Vector3 startPos = mainCamera.position;
        Quaternion startRot = mainCamera.rotation;

        while (timer < moveDuration)
        {
            timer += Time.deltaTime;
            float t = timer / moveDuration;
            t = t * t * (3f - 2f * t); 

            mainCamera.position = Vector3.Lerp(startPos, finalPos, t);
            mainCamera.rotation = Quaternion.Slerp(startRot, finalRot, t);
            yield return null;
        }

        // --- 阶段 3: 正式接管 ---
        
        // 3.1 恢复摄像机跟随
        if (cameraFollowScript != null)
        {
            cameraFollowScript.enabled = true;
            cameraFollowScript.ResetCamera(); 
        }

        // 3.2 恢复刚体物理模拟，让它受重力影响
        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = false;
            // 可选：刚恢复时可能会有极小的积攒速度，清零一下更保险
            playerRigidbody.velocity = Vector3.zero; 
        }

        // 3.3 恢复玩家控制脚本
        if (playerScript != null) playerScript.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (pauseMaster != null) pauseMaster.SetGameStarted(true);

        gameObject.SetActive(false);
    }
}