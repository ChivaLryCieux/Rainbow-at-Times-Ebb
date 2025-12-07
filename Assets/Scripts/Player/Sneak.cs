using UnityEngine;

public class PlayerSneak : MonoBehaviour
{
    [Header("潜行设置")]
    public KeyCode sneakKey = KeyCode.Mouse0; // 潜行按键（鼠标左键）
    public float sneakSpeed = 1f; // 潜行时的移动速度
    public float normalSpeed = 5f; // 正常移动速度

    [Header("引用")]
    public PlayerMovement controller; // 假设使用CharacterController2D控制移动
    // 如果用Rigidbody2D，替换为public Rigidbody2D rb;

    public bool isSneaking { get; private set; } // 公开的潜行状态（供NPC检测）

    void Update()
    {
        // 按下鼠标左键切换潜行状态（按住潜行，松开取消）
        if (Input.GetKeyDown(sneakKey))
        {
            isSneaking = true;
            SetSpeed(sneakSpeed); // 切换为潜行速度
        }
        if (Input.GetKeyUp(sneakKey))
        {
            isSneaking = false;
            SetSpeed(normalSpeed); // 恢复正常速度
        }
    }

    // 设置移动速度（根据你的移动组件类型选择实现）
    void SetSpeed(float speed)
    {
        // 如果你用CharacterController2D（如第三方控制器）
        if (controller != null)
        {
            controller.moveSpeed = speed;
        }

        // 如果你用Rigidbody2D
        // if (rb != null)
        // {
        //     // 这里假设你的移动逻辑中用一个speed变量控制，直接修改即可
        // }
    }

    // 可选：绘制潜行状态的Gizmo（场景中方便调试）
    void OnDrawGizmos()
    {
        if (isSneaking)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 1f); // 潜行时显示蓝色范围提示
        }
    }
}
