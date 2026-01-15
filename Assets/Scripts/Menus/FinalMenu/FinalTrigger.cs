using UnityEngine;
using UnityEngine.Video; // 【关键】必须引用 Video 命名空间

public class FinalTrigger : MonoBehaviour
{
    [Header("设置此结局对应的视频")]
    public VideoClip myEndingClip; // 【新增】在这里拖入不同的视频

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            Debug.Log("触发结局！");

            if (FinalMenuController.Instance != null)
            {
                // 【关键】把自己手里拿着的视频传给控制器
                FinalMenuController.Instance.StartEndingSequence(myEndingClip);
            }
            else
            {
                Debug.LogError("场景里没有 FinalMenuController！");
            }
        }
    }
}