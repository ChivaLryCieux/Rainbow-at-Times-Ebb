using UnityEngine;
using System.Collections.Generic;

public class MagneticMasterSwitch : MonoBehaviour
{
    [Header("连接设置")]
    [Tooltip("拖入要控制的装置。注意：现在踩一次切换一次状态。")]
    public List<UniversalMagneticField> targetDevices;

    [Header("触发设置")]
    public List<string> validTags = new List<string> { "Player", "Prop" };

    [Header("视觉反馈")]
    public float pressDistance = 0.05f;

    private int _objectsOnPlate = 0;
    private Vector3 _originalPos;

    // 增加一个防抖动标记，防止踩在边缘疯狂连跳
    private bool _isPressedDown = false;

    void Start()
    {
        _originalPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!validTags.Contains(other.tag)) return;

        _objectsOnPlate++;

        if (_objectsOnPlate == 1 && !_isPressedDown)
        {
            UpdateVisual(true);
            _isPressedDown = true;

            Debug.Log($"开关触发，列表共有 {targetDevices.Count} 个装置");

            // --- 这里的循环改写了，更安全 ---
            for (int i = 0; i < targetDevices.Count; i++)
            {
                var device = targetDevices[i];

                // 1. 检查是否为空
                if (device == null)
                {
                    Debug.LogWarning($"第 {i + 1} 个位置是空的 (None)，请检查 Inspector!");
                    continue; // 跳过这个，继续下一个
                }

                // 2. 尝试执行，如果出错捕获它，不影响下一个
                try
                {
                    Debug.Log($"正在切换第 {i + 1} 个: {device.name}");
                    device.ToggleWorkingState();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"第 {i + 1} 个装置 ({device.name}) 报错了，导致中断！错误信息: {e.Message}");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!validTags.Contains(other.tag)) return;

        _objectsOnPlate--;
        if (_objectsOnPlate < 0) _objectsOnPlate = 0;

        // 当所有物体离开，仅仅是让按钮弹起来，但不改变装置状态
        if (_objectsOnPlate == 0 && _isPressedDown)
        {
            UpdateVisual(false);
            _isPressedDown = false;
            // 这里不再调用 ToggleWorkingState
        }
    }

    private void UpdateVisual(bool isPressed)
    {
        float targetY = isPressed ? _originalPos.y - pressDistance : _originalPos.y;
        transform.position = new Vector3(_originalPos.x, targetY, _originalPos.z);
    }
}