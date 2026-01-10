using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject gameOverPanel;
    private CharacterController cc;
    
    // 假设你的移动脚本叫 PlayerMovement (请根据实际名字修改)
    private MonoBehaviour movementScript; 

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        // 获取移动脚本，比如 PlayerMovement
        // movementScript = GetComponent<PlayerMovement>(); 
        // 如果你不知道具体名字，也可以尝试通用的获取方式，或者直接拖拽赋值
    }

    public void Die()
    {
        // 1. 先把移动脚本禁用了！
        // 假设你的移动控制脚本在同一个物体上
        var moveScript = GetComponent<PlayerMovement>(); // 替换成你真实的脚本类名
        if (moveScript != null) moveScript.enabled = false;

        // 2. 禁用 CC
        if (cc != null) cc.enabled = false;

        // 3. 显示 UI (如果你都要跳转场景了，这个Panel其实只会出现一瞬间)
        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        // 4. 暂停时间
        Time.timeScale = 0f;

        // 5. 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}