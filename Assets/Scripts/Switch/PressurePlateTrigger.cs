using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateTrigger : MonoBehaviour
{
    [Header("连接")]
    [Tooltip("把门轴对象拖到这里")]
    public TrapdoorController targetDoor;

    [Header("视觉反馈 (可选)")]
    [Tooltip("开关按下的位移距离")]
    public float pressDistance = 0.05f;

    // 用于记录有多少个物体正压在开关上
    private int _objectsOnPlate = 0;
    private Vector3 _originalPos;

    void Start()
    {
        _originalPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 这里可以过滤特定的Tag，比如只允许 Player 或 Box 触发
        // if (!other.CompareTag("Player") && !other.CompareTag("Box")) return;

        _objectsOnPlate++;

        // 只有当这是第一个上来的物体时，才触发开门
        if (_objectsOnPlate == 1)
        {
            targetDoor.SetDoorState(true); // 开门
            UpdateVisual(true);            // 按钮下沉
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if (!other.CompareTag("Player") && !other.CompareTag("Box")) return;

        _objectsOnPlate--;

        // 防止计数器因为奇怪的物理碰撞变成负数
        if (_objectsOnPlate < 0) _objectsOnPlate = 0;

        // 只有当所有物体都离开时，才触发关门
        if (_objectsOnPlate == 0)
        {
            targetDoor.SetDoorState(false); // 关门复位
            UpdateVisual(false);            // 按钮回弹
        }
    }

    // 简单的视觉效果，让按钮看起来被踩下去了
    private void UpdateVisual(bool isPressed)
    {
        float targetY = isPressed ? _originalPos.y - pressDistance : _originalPos.y;
        transform.position = new Vector3(_originalPos.x, targetY, _originalPos.z);
    }
}