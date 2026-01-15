using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;

    // 改用平滑时间，更直观且不容易出错
    [Tooltip("平滑时间：数值越小越快，0.1~0.3 是不错的范围。绝对不能为0！")]
    public float smoothTime = 0.2f;

    [Header("位置偏移")]
    public Vector3 offset = new Vector3(0, 2, -10);

    // 速度缓存变量
    private float _xVelocity = 0f;

    // --- 重置方法 ---
    // 用于在开场动画结束、脚本重新启用时调用
    // 强制清除之前的速度残留，并确保位置对齐
    public void ResetCamera()
    {
        _xVelocity = 0f; // 清除平滑速度缓存
        
        if (target != null)
        {
            // 强制将摄像机瞬间移动到目标位置
            // 这样可以消除从 "开场动画位置" 到 "跟随计算位置" 之间可能存在的微小误差
            float targetX = target.position.x;
            transform.position = new Vector3(targetX, target.position.y + offset.y, target.position.z + offset.z);
        }
    }

    void Start()
    {
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- 保险 1：检查平滑时间 ---
        if (smoothTime <= 0.001f) smoothTime = 0.001f;

        // --- 保险 2：检查摄像机自身坐标是否已经坏掉 ---
        if (float.IsNaN(transform.position.x) || float.IsNaN(transform.position.y) || float.IsNaN(transform.position.z))
        {
            Debug.LogWarning("摄像机坐标出现 NaN，正在尝试自动修复...");
            transform.position = target.position + offset;
            _xVelocity = 0f; 
            return; 
        }

        // --- 保险 3：检查速度变量是否坏掉 ---
        if (float.IsNaN(_xVelocity))
        {
            _xVelocity = 0f;
        }

        // --- 正常计算逻辑 ---
        float targetX = target.position.x;

        // 使用 smoothTime 计算平滑后的 X 坐标
        float currentX = Mathf.SmoothDamp(transform.position.x, targetX, ref _xVelocity, smoothTime);

        // 防止计算结果出现 NaN
        if (float.IsNaN(currentX))
        {
            currentX = targetX;
            _xVelocity = 0f;
        }

        // 应用坐标 (注意：Y 和 Z 是硬绑定，只平滑 X)
        transform.position = new Vector3(currentX, target.position.y + offset.y, target.position.z + offset.z);
    }
}