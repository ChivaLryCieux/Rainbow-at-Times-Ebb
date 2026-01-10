using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("移动的偏移量 (例如 Y=5 代表向上移动5米)")]
    public Vector3 moveOffset = new Vector3(0, 5, 0);

    [Tooltip("移动速度")]
    public float speed = 3.0f;

    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        // 记录起点
        startPos = transform.position;
        // 计算终点
        endPos = startPos + moveOffset;
    }

    void FixedUpdate()
    {
        // 计算插值 (0 到 1 之间往复变化)
        // Mathf.PingPong 会产生一个在 0 和 1 之间像乒乓球一样来回弹跳的值
        float t = Mathf.PingPong(Time.time * speed / moveOffset.magnitude, 1.0f);

        // 在起点和终点之间平滑移动
        transform.position = Vector3.Lerp(startPos, endPos, t);
    }

    // --- 载人逻辑：让 CharacterController 跟着平台动 ---

    private void OnTriggerEnter(Collider other)
    {
        // 当玩家站到 Trigger 区域内
        if (other.CompareTag("Player"))
        {
            // 把玩家设置为平台的“子物体”
            // 这样平台移动时，作为子物体的玩家也会自动跟着移动
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 当玩家离开 Trigger 区域
        if (other.CompareTag("Player"))
        {
            // 解除父子关系，让玩家恢复自由
            other.transform.SetParent(null);

            // 注意：如果你的场景里有 DontDestroyOnLoad 的根节点
            // 或者特定的层级管理，这里可能需要 SetParent(原来的父节点)
        }
    }
}