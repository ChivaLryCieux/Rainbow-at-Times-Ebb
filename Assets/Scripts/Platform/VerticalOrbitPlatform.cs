using UnityEngine;

public class VerticalOrbitPlatform : MonoBehaviour
{
    // 定义枚举：选择旋转平面
    public enum OrbitPlane
    {
        XY_FrontView, // 在 XY 平面旋转（像是面对你的摩天轮）
        ZY_SideView   // 在 ZY 平面旋转（像是侧对着你的摩天轮）
    }

    [Header("核心设置")]
    [Tooltip("旋转中心点")]
    public Transform centerPoint;

    [Tooltip("选择旋转的平面")]
    public OrbitPlane rotationPlane = OrbitPlane.XY_FrontView;

    [Header("运动参数")]
    public float radius = 5.0f;     // 半径
    public float speed = 45.0f;     // 速度
    public bool isClockwise = true; // 方向

    // 内部变量
    private float _currentAngle;

    void Start()
    {
        if (centerPoint == null)
        {
            Debug.LogError("请分配中心点 (Center Point)！");
            return;
        }

        // --- 智能初始化角度 ---
        // 根据当前的相对位置计算初始角度，防止瞬移
        Vector3 offset = transform.position - centerPoint.position;

        if (rotationPlane == OrbitPlane.XY_FrontView)
        {
            // XY平面：计算 Y 和 X 的夹角
            _currentAngle = Mathf.Atan2(offset.y, offset.x);
        }
        else
        {
            // ZY平面：计算 Y 和 Z 的夹角
            _currentAngle = Mathf.Atan2(offset.y, offset.z);
        }
    }

    void FixedUpdate()
    {
        if (centerPoint == null) return;

        // 1. 更新角度
        float direction = isClockwise ? -1f : 1f;
        _currentAngle += speed * Mathf.Deg2Rad * direction * Time.fixedDeltaTime;

        // 2. 计算坐标
        float x = centerPoint.position.x;
        float y = centerPoint.position.y;
        float z = centerPoint.position.z;

        // 核心数学公式：根据平面修改不同的轴
        if (rotationPlane == OrbitPlane.XY_FrontView)
        {
            // 改变 X 和 Y，Z 保持圆心的深度
            x += Mathf.Cos(_currentAngle) * radius;
            y += Mathf.Sin(_currentAngle) * radius;
        }
        else // ZY_SideView
        {
            // 改变 Z 和 Y，X 保持圆心的位置
            z += Mathf.Cos(_currentAngle) * radius;
            y += Mathf.Sin(_currentAngle) * radius;
        }

        // 3. 应用位置
        transform.position = new Vector3(x, y, z);
    }

    // --- 外部接口 ---
    public void ToggleDirection()
    {
        isClockwise = !isClockwise;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    // --- 玩家粘滞 (必不可少) ---
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) other.transform.SetParent(null);
    }

    // --- 编辑器辅助线 (可视化圆环) ---
    private void OnDrawGizmos()
    {
        if (centerPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(centerPoint.position, 0.3f); // 画圆心
            Gizmos.DrawLine(centerPoint.position, transform.position); // 画连接杆

            // 画出竖直的轨道
            Vector3 prevPos = GetOrbitPosition(0);
            for (int i = 1; i <= 36; i++)
            {
                Vector3 nextPos = GetOrbitPosition(i * 10 * Mathf.Deg2Rad);
                Gizmos.DrawLine(prevPos, nextPos);
                prevPos = nextPos;
            }
        }
    }

    // 辅助计算函数 (仅用于 Gizmos 画线)
    Vector3 GetOrbitPosition(float angle)
    {
        float x = centerPoint.position.x;
        float y = centerPoint.position.y;
        float z = centerPoint.position.z;

        if (rotationPlane == OrbitPlane.XY_FrontView)
        {
            x += Mathf.Cos(angle) * radius;
            y += Mathf.Sin(angle) * radius;
        }
        else
        {
            z += Mathf.Cos(angle) * radius;
            y += Mathf.Sin(angle) * radius;
        }
        return new Vector3(x, y, z);
    }
}