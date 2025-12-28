using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("请将包含'游戏结束'文字的UI物体拖到这里")]
    public GameObject gameOverUI; 

    private void OnTriggerEnter(Collider other)
    {
        // 检测是否是玩家
        if (other.CompareTag("Player"))
        {
            GameOver(other.gameObject);
        }
    }

    void GameOver(GameObject player)
    {
        Debug.Log("玩家死亡，游戏结束！");

        // 1. 显示 UI
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // 2. 针对 CharacterController 的停止方法
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            // 简单粗暴：直接禁用角色控制器组件，玩家就卡住动不了了
            cc.enabled = false; 
        }

        // 3. 停止时间 (强烈推荐)
        // 这是最保险的方法，无论你用什么方式移动，时间停了大家都得停
        Time.timeScale = 0f;

        // 4. 解锁鼠标 (方便点击UI)
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}