using UnityEngine;

public class GravitySwitch : MonoBehaviour
{
    [Header("连接")]
    [Tooltip("拖入要控制的重力区域")]
    public GravityControlZone targetZone;

    [Header("视觉反馈")]
    public float pressDistance = 0.05f;

    private bool _isPressed = false;
    private Vector3 _originalPos;

    void Start()
    {
        _originalPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 只有主角能触发
        if (other.CompareTag("Player") && !_isPressed)
        {
            _isPressed = true;
            UpdateVisual(true);

            if (targetZone != null)
            {
                // 切换重力方向
                targetZone.ToggleGravity();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _isPressed)
        {
            _isPressed = false;
            UpdateVisual(false);
        }
    }

    private void UpdateVisual(bool pressed)
    {
        float targetY = pressed ? _originalPos.y - pressDistance : _originalPos.y;
        transform.position = new Vector3(_originalPos.x, targetY, _originalPos.z);
    }
}