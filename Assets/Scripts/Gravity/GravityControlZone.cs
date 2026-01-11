using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class GravityControlZone : MonoBehaviour
{
    [Header("区域设置 (自动生成)")]
    public Transform pointA; // 端点 A
    public Transform pointB; // 端点 B

    [Header("重力设置")]
    [Tooltip("反向重力的强度。注意：这需要大到足以抵消你原本的下落速度。")]
    public float reverseForce = 25f; // 经验值：通常设为 20-30 才能抵消默认重力并上升

    [Header("当前状态")]
    public bool isGravityReversed = false;

    // 内部变量
    private BoxCollider _zoneCollider;
    private PlayerGravityReceiver _playerReceiver;

    void Start()
    {
        _zoneCollider = GetComponent<BoxCollider>();
        _zoneCollider.isTrigger = true;

        // 如果设定了两点，自动计算 BoxCollider 的大小和位置
        if (pointA != null && pointB != null)
        {
            UpdateZoneFromPoints();
        }
    }

    // --- 供开关调用：切换重力状态 ---
    public void ToggleGravity()
    {
        isGravityReversed = !isGravityReversed;
        Debug.Log($"重力区域状态切换: {(isGravityReversed ? "反转 (向上)" : "正常 (向下)")}");

        // 如果主角此时就在区域内，立即更新他的状态
        if (_playerReceiver != null)
        {
            _playerReceiver.SetGravityState(isGravityReversed);
        }
    }

    // --- 物理逻辑 ---

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 获取主角身上的接收器
            _playerReceiver = other.GetComponent<PlayerGravityReceiver>();
            if (_playerReceiver != null)
            {
                // 进入区域时，将主角状态同步为当前区域状态
                _playerReceiver.SetGravityState(isGravityReversed);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_playerReceiver != null)
            {
                // 离开区域时，强制恢复正常重力
                _playerReceiver.SetGravityState(false);
                _playerReceiver = null;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 只有当重力反转，且主角在区域内时，才施加“上浮力”
        if (isGravityReversed && _playerReceiver != null)
        {
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                // 持续施加向上的力
                // 注意：这里使用 Vector3.up (世界坐标)，不管主角怎么旋转，重力总是反向于世界
                cc.Move(Vector3.up * reverseForce * Time.deltaTime);
            }
        }
    }

    // --- 工具：根据两点计算区域 ---
    [ContextMenu("根据两点更新区域")]
    public void UpdateZoneFromPoints()
    {
        if (pointA == null || pointB == null) return;

        // 计算中心点
        transform.position = (pointA.position + pointB.position) / 2f;

        // 计算两点之间的向量差
        Vector3 diff = pointB.position - pointA.position;

        // 设置 BoxCollider 大小 (取绝对值)
        // 注意：这会生成一个最简长方体。如果需要更精确的旋转长方体，需要调整 transform.rotation
        _zoneCollider.size = new Vector3(Mathf.Abs(diff.x) + 1f, Mathf.Abs(diff.y) + 1f, Mathf.Abs(diff.z) + 1f);
    }

    private void OnDrawGizmos()
    {
        // 画出区域轮廓
        Gizmos.color = isGravityReversed ? new Color(1, 0, 1, 0.3f) : new Color(0, 1, 0, 0.2f);
        if (_zoneCollider == null) _zoneCollider = GetComponent<BoxCollider>();

        if (_zoneCollider != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_zoneCollider.center, _zoneCollider.size);
            Gizmos.DrawWireCube(_zoneCollider.center, _zoneCollider.size);
        }
    }
}