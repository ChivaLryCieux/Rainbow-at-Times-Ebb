using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("设置")]
    public int checkpointID; // 【重要】请在编辑器里手动填 1, 2, 3... (出生点不用挂这个脚本)

    [Header("视觉效果")]
    public Light glowLight; // 拖入子物体的 Point Light
    public bool isActivated = false;

    private void Start()
    {
        // 1. 向 GameManager 注册自己，方便引导光束找到我
        GameManager.Instance.RegisterCheckpoint(this);

        // 2. 如果玩家已经存档过这个点（比如读档回来），直接关灯
        if (GameManager.Instance.MaxIndex >= checkpointID)
        {
            DisableGlow();
            isActivated = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActivated)
        {
            isActivated = true;
            DisableGlow(); // 关灯

            // 调用 GameManager 保存
            GameManager.Instance.AddCheckpoint(transform.position);
        }
    }

    private void DisableGlow()
    {
        if (glowLight != null)
        {
            glowLight.enabled = false; // 或者用动画渐隐
        }
    }
}