using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class GravityZone : MonoBehaviour
{
    [Header("区域范围设置")]
    public Transform pointA; // 端点 A
    public Transform pointB; // 端点 B

    [Header("力学设置")]
    [Tooltip("向上的推力。必须大于你的重力设置 (建议 20 ~ 40)")]
    public float upwardForce = 30f;

    // --- 核心：引用计数器 ---
    // 0 = 正常, >0 = 反转
    private int _activeSwitchCount = 0;

    private BoxCollider _zoneCollider;

    void Start()
    {
        _zoneCollider = GetComponent<BoxCollider>();
        _zoneCollider.isTrigger = true;

        // 自动设置区域大小
        if (pointA != null && pointB != null)
        {
            UpdateZoneFromPoints();
        }
    }

    // --- 供开关调用 ---

    // 开关踩下
    public void AddActivation()
    {
        _activeSwitchCount++;
        Debug.Log($"区域激活数: {_activeSwitchCount} (重力反转)");
    }

    // 开关弹起
    public void RemoveActivation()
    {
        _activeSwitchCount--;
        if (_activeSwitchCount < 0) _activeSwitchCount = 0;
        Debug.Log($"区域激活数: {_activeSwitchCount} ({(_activeSwitchCount > 0 ? "保持反转" : "恢复正常")})");
    }

    // --- 物理逻辑 ---

    private void OnTriggerStay(Collider other)
    {
        // 只有当至少有 1 个开关被踩下，且对象是玩家时
        if (_activeSwitchCount > 0 && other.CompareTag("Player"))
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                // 直接施加向上的移动
                // 这会和你原本的下落重力叠加。
                // 只要 upwardForce > 你的重力，玩家就会上浮。
                cc.Move(Vector3.up * upwardForce * Time.deltaTime);
            }
        }
    }

    // --- 辅助工具 ---
    [ContextMenu("根据两点更新区域")]
    public void UpdateZoneFromPoints()
    {
        if (pointA == null || pointB == null) return;

        transform.position = (pointA.position + pointB.position) / 2f;
        Vector3 diff = pointB.position - pointA.position;
        // 加一点额外的填充(0.5f)防止边缘判定问题
        _zoneCollider.size = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
    }

    private void OnDrawGizmos()
    {
        // 激活时显示洋红色，未激活显示绿色
        Gizmos.color = _activeSwitchCount > 0 ? new Color(1, 0, 1, 0.3f) : new Color(0, 1, 0, 0.2f);

        if (_zoneCollider == null) _zoneCollider = GetComponent<BoxCollider>();
        if (_zoneCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_zoneCollider.center, _zoneCollider.size);
            Gizmos.DrawWireCube(_zoneCollider.center, _zoneCollider.size);
        }
    }
}