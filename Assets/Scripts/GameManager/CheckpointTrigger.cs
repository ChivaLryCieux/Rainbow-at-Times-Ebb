using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    // 是否已经触发过（防止玩家重复触发同一个点播放音效等）
    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // 只有玩家才能触发
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;

            // 调用管理器的保存方法，传入当前存档点的位置
            GameManager.Instance.AddCheckpoint(transform.position);

            Debug.Log("到达存档点！");
        }
    }
}