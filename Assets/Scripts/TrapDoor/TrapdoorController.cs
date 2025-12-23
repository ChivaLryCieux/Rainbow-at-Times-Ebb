using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapdoorController : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("旋转速度 (度/秒)")]
    public float rotationSpeed = 90f;

    [Tooltip("开启时旋转的角度 (顺时针90度填 -90)")]
    public float openAngle = -90f;

    // 内部变量记录状态
    private Quaternion _closedRotation;
    private Quaternion _openRotation;
    private Quaternion _targetRotation;

    void Start()
    {
        // 1. 记录游戏开始时门的状态为“关闭状态”
        _closedRotation = transform.localRotation;

        // 2. 预先计算好“开启状态”的四元数
        // 这样可以保证每次旋转的终点绝对精确，不会有累积误差
        _openRotation = _closedRotation * Quaternion.Euler(0, 0, openAngle);

        // 3. 初始目标设为关闭
        _targetRotation = _closedRotation;
    }

    void Update()
    {
        // 每一帧都尝试向“目标状态”旋转
        // Quaternion.RotateTowards 会自动处理最短路径，并且到达目标后自动停止
        // 这里的 0.1f 是一个极小的容差，防止无限逼近但不停止
        if (Quaternion.Angle(transform.localRotation, _targetRotation) > 0.1f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                _targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // 公开方法：设置门的状态
    public void SetDoorState(bool isOpen)
    {
        if (isOpen)
        {
            _targetRotation = _openRotation;
        }
        else
        {
            _targetRotation = _closedRotation;
        }
    }
}