using UnityEngine;

public class MagneticWallCC : MonoBehaviour
{
    [Header("吸力设置")]
    [Tooltip("吸附速度 (米/秒) - 数值越大吸得越快")]
    public float suctionSpeed = 10f;

    [Tooltip("是否根据距离衰减？(离得越近吸力越大)")]
    public bool useDistanceFalloff = true;

    [Tooltip("主角的Tag")]
    public string targetTag = "Player";

   

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
                // 3. 计算方向：从主角指向墙壁中心
                Vector3 direction = (transform.position - other.transform.position).normalized;

                // 4. 计算最终移动速度
                float currentSpeed = suctionSpeed;

                if (useDistanceFalloff)
                {
                    float distance = Vector3.Distance(transform.position, other.transform.position);
                    // 距离越近，速度越快 (加个 1.0f 防止除以0)
                    currentSpeed = suctionSpeed / Mathf.Max(distance * 0.5f, 0.5f);
                }

                // 5. 核心区别：直接调用 Move 方法
                // 注意：这会和主角自身的移动脚本叠加
                cc.Move(direction * currentSpeed * Time.deltaTime);
            }
        }
    }
}
