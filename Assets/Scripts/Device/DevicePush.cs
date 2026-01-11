using UnityEngine;

public class MagneticWall : MonoBehaviour
{
    [Header("推力设置")]
    [Tooltip("推动速度 (米/秒) - 数值越大推动得越快")]
    public float pullSpeed = 10f;

    [Tooltip("是否根据距离增强？(离得越近推力越大)")]
    public bool useForceRampUp = true;

    [Tooltip("主角的Tag")]
    public string targetTag = "Player";

    [Header("调试")]
    public bool showGizmos = true;

    // 当主角停留在触发器内部时每帧调用
    private void OnTriggerStay(Collider other)
    {
        // 1. 检查Tag
        if (other.CompareTag(targetTag))
        {
            // 2. 获取 CharacterController 组件
            CharacterController cc = other.GetComponent<CharacterController>();

            if (cc != null)
            {
                // 3. 计算方向：从墙壁中心指向主角 (实现推开效果)
                Vector3 direction = (other.transform.position - transform.position).normalized;

                // 4. 计算最终移动速度
                float currentSpeed = pullSpeed;

                if (useForceRampUp)
                {
                    float distance = Vector3.Distance(transform.position, other.transform.position);
                    // 距离越近，速度越快 (加个 1.0f 防止除以0)
                    currentSpeed = pullSpeed / Mathf.Max(distance * 0.5f, 0.5f);
                }

                // 5. 核心区别：直接调用 Move 方法
                // 注意：这会和主角自身的移动脚本叠加
                cc.Move(direction * currentSpeed * Time.deltaTime);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!this.enabled) return;
        if (showGizmos)
        {
            Gizmos.color = new Color(1, 0.5f, 0, 0.3f); // 橙色以示区别
            BoxCollider box = GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawCube(box.center, box.size);
            }
        }
    }
}