using UnityEngine;

public class AutoMovingPlatform : MonoBehaviour
{
    [Header("路径设置")]
    [Tooltip("拖入一个空物体作为起点 A")]
    public Transform pointA;

    [Tooltip("拖入一个空物体作为终点 B")]
    public Transform pointB;

    [Header("运动参数")]
    [Tooltip("移动速度")]
    public float speed = 3.0f;

    [Tooltip("到达端点后的停留时间 (秒)")]
    public float waitTime = 1.0f;

    private Vector3 _targetPos;
    private float _waitTimer = 0f;
    private bool _isWaiting = false;

    void Start()
    {
        // 容错：如果没有设置点，就以当前位置为 A，当前位置+向上3米为 B
        if (pointA == null || pointB == null)
        {
            Debug.LogError("请在 Inspector 面板中分配 Point A 和 Point B！");
            return;
        }

        // 初始目标设为 B
        _targetPos = pointB.position;

        // 为了防止开始时跳台瞬移，建议在编辑器里就把跳台放在 A 点的位置
        transform.position = pointA.position;
    }

    void FixedUpdate()
    {
        if (pointA == null || pointB == null) return;

        // 如果正在等待，就倒计时
        if (_isWaiting)
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= waitTime)
            {
                _isWaiting = false;
                _waitTimer = 0;
            }
            return; // 等待期间不移动
        }

        // --- 核心移动逻辑 ---
        // 使用 MoveTowards 保证匀速运动
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, speed * Time.fixedDeltaTime);

        // --- 检查是否到达目标点 ---
        // Vector3.Distance 检查两者距离是否极小
        if (Vector3.Distance(transform.position, _targetPos) < 0.01f)
        {
            // 到达了！开始等待
            _isWaiting = true;

            // 切换目标：如果当前目标是 A，下一次就去 B；反之亦然
            if (_targetPos == pointA.position)
            {
                _targetPos = pointB.position;
            }
            else
            {
                _targetPos = pointA.position;
            }
        }
    }

    // --- 玩家粘滞逻辑 (防止滑落) ---
    // 必须保留，因为 CharacterController 不会自动跟随移动的碰撞体
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
            // 提示：如果你的场景有 DontDestroyOnLoad 或者复杂的层级，
            // 这里可能需要 SetParent(原来的父对象)
        }
    }
}