using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    private Animator _animator;
    private PlayerMovement _movement; // 引用刚才那个脚本

    // 缓存参数ID，比直接用字符串效率高
    private int _speedHash = Animator.StringToHash("Speed");
    private int _groundHash = Animator.StringToHash("IsGrounded");
    private int _jumpHash = Animator.StringToHash("Jump");

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _movement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        // 订阅事件：当 movement 脚本发生 Jump 时，执行 PlayJump 动画
        if (_movement != null)
            _movement.OnJump += PlayJump;
    }

    void OnDisable()
    {
        // 养成好习惯：脚本禁用时取消订阅，防止报错
        if (_movement != null)
            _movement.OnJump -= PlayJump;
    }

    void Update()
    {
        if (_movement == null) return;

        // 1. 同步速度 (0 -> 站立, >0.1 -> 跑)
        // 我们读取刚才写的 GetCurrentSpeed 方法
        _animator.SetFloat(_speedHash, _movement.GetCurrentSpeed());

        // 2. 同步地面状态
        // 读取刚才公开的 IsGrounded 属性
        _animator.SetBool(_groundHash, _movement.IsGrounded);
    }

    // 这个方法会在收到 OnJump 广播时自动执行
    private void PlayJump()
    {
        _animator.SetTrigger(_jumpHash);
    }
}