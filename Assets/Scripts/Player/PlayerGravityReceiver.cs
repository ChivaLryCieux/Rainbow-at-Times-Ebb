using UnityEngine;

// 将此脚本挂载在主角 (Player) 身上
public class PlayerGravityReceiver : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("翻转的速度")]
    public float rotationSpeed = 5f;

    // 目标上下方向 (默认是世界坐标的上方)
    private Vector3 _targetUp = Vector3.up;
    
    // 记录是否处于反重力状态
    public bool IsGravityReversed { get; private set; } = false;

    void Update()
    {
        // 平滑旋转主角，使其头顶指向 _targetUp
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, _targetUp) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    // 供外部区域调用：设置重力方向
    public void SetGravityState(bool reversed)
    {
        IsGravityReversed = reversed;
        // 如果反转，头朝下(Vector3.down)；否则头朝上(Vector3.up)
        _targetUp = reversed ? Vector3.down : Vector3.up;
    }
}