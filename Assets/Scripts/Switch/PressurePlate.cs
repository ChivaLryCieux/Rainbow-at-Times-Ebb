using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [Header("核心连接")]
    public TrapdoorController targetDoor;

    [Header("触发设置")]
    [Tooltip("只允许这个Tag的物体触发")]
    public string allowedTag = "Ball";

    [Tooltip("离开后的延迟复位时间 (秒)")]
    public float releaseDelay = 0.5f; // 0.5秒的缓冲期足以消除大部分抖动

    [Header("视觉反馈")]
    public float pressDistance = 0.05f;
    public float pressSpeed = 10f;

    // 内部状态
    private Vector3 _upPos;
    private Vector3 _downPos;
    private Vector3 _targetVisualPos;

    private List<Collider> _collidersOnButton = new List<Collider>();

    // 延迟控制变量
    private float _lastEmptyTime = -1f; // 上一次变空的时间点
    private bool _isPendingReset = false; // 是否正在等待复位

    void Start()
    {
        _upPos = transform.position;
        _downPos = _upPos - new Vector3(0, pressDistance, 0);
        _targetVisualPos = _upPos;
    }

    void Update()
    {
        // 1. 列表自动清理 (处理物体被销毁的情况)
        CleanUpList();

        // 2. 状态判断逻辑
        bool shouldBeOpen = false;

        if (_collidersOnButton.Count > 0)
        {
            // A. 如果有东西在上面 -> 必须开
            shouldBeOpen = true;
            _isPendingReset = false; // 取消任何复位倒计时
        }
        else
        {
            // B. 如果列表空了 (东西离开了)
            if (!_isPendingReset)
            {
                // 刚变空的一瞬间，开始计时
                _isPendingReset = true;
                _lastEmptyTime = Time.time;
                shouldBeOpen = true; // 暂时保持开启（缓冲期）
            }
            else
            {
                // 已经在等待中，检查时间到了没
                if (Time.time < _lastEmptyTime + releaseDelay)
                {
                    shouldBeOpen = true; // 还没到时间，继续保持开启
                }
                else
                {
                    shouldBeOpen = false; // 时间到了，真正关闭
                }
            }
        }

        // 3. 执行状态应用
        UpdateState(shouldBeOpen);

        // 4. 视觉平滑移动
        if (Vector3.Distance(transform.position, _targetVisualPos) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, _targetVisualPos, Time.deltaTime * pressSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // --- 过滤逻辑 ---
        if (!other.CompareTag(allowedTag)) return;
        // ---------------

        if (!_collidersOnButton.Contains(other))
        {
            _collidersOnButton.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // --- 过滤逻辑 ---
        if (!other.CompareTag(allowedTag)) return;
        // ---------------

        if (_collidersOnButton.Contains(other))
        {
            _collidersOnButton.Remove(other);
        }
    }

    // 辅助方法：清理无效物体
    private void CleanUpList()
    {
        for (int i = _collidersOnButton.Count - 1; i >= 0; i--)
        {
            if (_collidersOnButton[i] == null ||
                !_collidersOnButton[i].gameObject.activeInHierarchy ||
                !_collidersOnButton[i].enabled)
            {
                _collidersOnButton.RemoveAt(i);
            }
        }
    }

    private void UpdateState(bool isOpen)
    {
        if (isOpen)
        {
            _targetVisualPos = _downPos;
            if (targetDoor != null) targetDoor.SetDoorState(true);
        }
        else
        {
            _targetVisualPos = _upPos;
            if (targetDoor != null) targetDoor.SetDoorState(false);
        }
    }
}