using UnityEngine;

public class PuzzleCharacter : MonoBehaviour
{
    [Tooltip("当前角色是否允许被玩家控制")]
    public bool isControlled = false;

    // 这里不再包含 Update、Move 或任何修改 Transform/Velocity 的代码
    // 它的唯一作用就是作为一个“令牌”，供你的移动脚本和 TrapManager 读取
}