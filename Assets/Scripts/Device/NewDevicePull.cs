using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class UniversalMagneticField : MonoBehaviour
{
    [Header("初始状态设置")]
    public bool defaultActive = true;

    [Header("目标设置")]
    public List<string> targetTags = new List<string> { "Player", "Prop", "Enemy" };

    [Header("力学设置")]
    public float rigidBodyForce = 50f;     // 刚体受到的力
    public float characterSpeed = 5f;      // 角色移动速度
    public bool useDistanceFalloff = true; // 距离衰减

    [Header("调试")]
    public bool showGizmos = true;

    private void Awake()
    {
        // 初始化状态
        SetEnabledState(defaultActive);
    }

    // --- 供开关调用 ---
    public void ToggleWorkingState()
    {
        // 仅仅是切换开/关，不做任何物理干涉
        SetEnabledState(!this.enabled);
        Debug.Log($"装置 {gameObject.name} 状态切换为: {this.enabled}");
    }

    // 辅助函数：同时处理脚本和碰撞体的开关
    private void SetEnabledState(bool state)
    {
        this.enabled = state;
        if (GetComponent<Collider>() != null)
        {
            GetComponent<Collider>().enabled = state;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // 1. 检查Tag
        if (targetTags.Contains(other.tag))
        {
            // 2. 计算基础数据
            Vector3 direction = (transform.position - other.transform.position).normalized;
            float distance = Vector3.Distance(transform.position, other.transform.position);

            // 3. 计算力度衰减
            float intensityMultiplier = 1f;
            if (useDistanceFalloff)
            {
                intensityMultiplier = 1f / Mathf.Max(distance * 0.5f, 0.5f);
            }

            // 4. 应用纯物理力
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                // 只管推，不管停。
                // 当代码不运行(enabled=false)时，这里不再执行，
                // 物体会依然保留刚才的 velocity (速度)，表现为惯性。
                rb.AddForce(direction * rigidBodyForce * intensityMultiplier, ForceMode.Force);
            }
            else if (other.TryGetComponent<CharacterController>(out CharacterController cc))
            {
                cc.Move(direction * characterSpeed * intensityMultiplier * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!this.enabled) return;

        if (showGizmos)
        {
            Gizmos.color = new Color(1, 0.92f, 0.016f, 0.2f);
            Collider col = GetComponent<Collider>();
            if (col is BoxCollider box)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawSphere(sphere.center, sphere.radius);
            }
        }
    }
}