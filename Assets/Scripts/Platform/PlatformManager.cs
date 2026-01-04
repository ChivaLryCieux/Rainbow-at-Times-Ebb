using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour
{
    public GameObject[] platforms; // 跳台数组
    public float interval = 0.5f;  // 出现间隔
    public float duration = 5.0f;  // 存活时间

    void Start()
    {
        // 游戏开始时隐藏所有跳台
        HideAll();
        Debug.Log("[管理器] 初始化完成，所有跳台已隐藏。");
    }

    public void StartSequence(PlatformSwitch sourceSwitch)
    {
        StartCoroutine(SequenceRoutine(sourceSwitch));
    }

    IEnumerator SequenceRoutine(PlatformSwitch sourceSwitch)
    {
        Debug.Log("[管理器] 开始执行跳台生成序列...");

        // 1. 依次显示跳台
        for (int i = 0; i < platforms.Length; i++)
        {
            if (platforms[i] != null)
            {
                platforms[i].SetActive(true);
                Debug.Log($"[管理器] 显示第 {i + 1} 个跳台: {platforms[i].name}");
            }
            yield return new WaitForSeconds(interval);
        }

        Debug.Log($"[管理器] 所有跳台显示完毕，玩家有 {duration} 秒的时间...");

        // 2. 等待玩家跑酷时间
        yield return new WaitForSeconds(duration);

        // 3. 时间到，重置
        Debug.Log("[管理器] 时间到！隐藏跳台并重置开关。");
        HideAll();

        // 通知开关接触锁定
        if (sourceSwitch != null)
        {
            sourceSwitch.ResetButton();
        }
    }

    void HideAll()
    {
        foreach (var p in platforms)
        {
            if (p != null) p.SetActive(false);
        }
    }
}