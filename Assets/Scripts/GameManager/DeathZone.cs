using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // 防止重复触发
    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        // 只要碰到的是玩家，且还没触发过
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;

            // 1. 尝试禁用玩家的移动输入，防止坠落时还能空中漫步
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            // 2. 直接呼叫 OverMenu，开始死亡表演
            if (OverMenuController.Instance != null)
            {
                Debug.Log("玩家坠入死亡区，触发 Game Over");
                OverMenuController.Instance.TriggerGameOver();
            }
            else
            {
                Debug.LogError("未找到 OverMenuController！请检查场景中是否挂载该脚本。");
            }
        }
    }
}