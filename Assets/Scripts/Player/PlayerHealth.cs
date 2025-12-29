using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject gameOverPanel; // 死亡弹窗

    private CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // 提供一个公开的方法供外界调用
    public void Die()
    {
        Debug.Log("玩家生命归零！");

        // 1. 禁用 CharacterController (防止尸体滑行或无法重生)
        if (cc != null)
        {
            cc.enabled = false;
        }

        // 2. 显示死亡界面
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 3. 暂停时间 (记得在GameManager里恢复)
        Time.timeScale = 0f;

        // 4. 解锁鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // (进阶) 如果有死亡动画，可以在这里播放
        // GetComponent<Animator>().SetTrigger("Die");
    }
}