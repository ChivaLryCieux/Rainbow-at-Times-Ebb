using UnityEngine;

public class PhantomTrapManager : MonoBehaviour
{
    [Header("角色引用")]
    public PuzzleCharacter playerChar;
    public PuzzleCharacter phantomChar;

    [Header("摄像机")]
    public GameObject mainCamera;
    public GameObject phantomCamera;

    [Header("场景物体")]
    public GameObject trapCage;          // 笼子对象
    public Transform phantomSpawnPoint;

    // 内部状态
    private bool isTrapActive = false;

    // 【修改点 1】 将初始化放入 Awake，确保最优先执行
    void Awake()
    {
        InitializeState();
    }

    private void InitializeState()
    {
        // 防空指针检查（防止你忘记拖拽物体报错）
        if (trapCage != null)
            trapCage.SetActive(false); // 【核心】强制隐藏笼子

        if (phantomChar != null)
        {
            phantomChar.gameObject.SetActive(false); // 强制隐藏幽灵
            phantomChar.isControlled = false;
        }

        if (playerChar != null)
            playerChar.isControlled = true;

        if (mainCamera != null) mainCamera.SetActive(true);
        if (phantomCamera != null) phantomCamera.SetActive(false);

        isTrapActive = false;
    }

    // --- 阶段一：触发陷阱 ---
    public void ActivateTrap()
    {
        if (isTrapActive) return;
        isTrapActive = true;

        Debug.Log("陷阱触发！");

        if (playerChar != null) playerChar.isControlled = false;

        // 【核心】在这里显示笼子
        if (trapCage != null) trapCage.SetActive(true);

        if (mainCamera != null) mainCamera.SetActive(false);
        if (phantomCamera != null) phantomCamera.SetActive(true);

        if (phantomChar != null)
        {
            phantomChar.transform.position = phantomSpawnPoint.position;
            phantomChar.transform.rotation = phantomSpawnPoint.rotation;
            phantomChar.gameObject.SetActive(true);
            phantomChar.isControlled = true;
        }
    }

    // --- 阶段二：解除陷阱 ---
    public void DeactivateTrap()
    {
        if (!isTrapActive) return;
        isTrapActive = false;

        Debug.Log("陷阱解除！");

        if (trapCage != null) trapCage.SetActive(false); // 再次隐藏笼子

        if (phantomChar != null)
        {
            phantomChar.isControlled = false;
            phantomChar.gameObject.SetActive(false);
        }

        if (phantomCamera != null) phantomCamera.SetActive(false);
        if (mainCamera != null) mainCamera.SetActive(true);

        if (playerChar != null) playerChar.isControlled = true;

        // 自动存档
        if (GameManager.Instance != null && playerChar != null)
        {
            GameManager.Instance.AddCheckpoint(playerChar.transform.position);
        }
    }
}