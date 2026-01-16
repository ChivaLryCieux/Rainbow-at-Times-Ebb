using UnityEngine;

public class TrapdoorController : MonoBehaviour
{
    [Header("设置")]
    public float rotateSpeed = 120f;
    // 门打开时的角度偏移量 (比如 -90 度)
    public float openAngleOffset = -90f;

    private Quaternion _closedRot;
    private Quaternion _openRot;
    private Quaternion _targetRot;

    void Start()
    {
        _closedRot = transform.localRotation;
        // 预先计算好开启状态的四元数，避免累积误差
        _openRot = _closedRot * Quaternion.Euler(0, 0, openAngleOffset);
        _targetRot = _closedRot;
    }

    void Update()
    {
        // 始终向目标角度旋转
        transform.localRotation = Quaternion.RotateTowards(
            transform.localRotation,
            _targetRot,
            rotateSpeed * Time.deltaTime
        );
    }

    // 设置门的状态：true=开, false=关
    public void SetDoorState(bool isOpen)
    {
        _targetRot = isOpen ? _openRot : _closedRot;
    }
}