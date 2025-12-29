using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. 判断是不是玩家
        if (other.CompareTag("Player"))
        {
            // 2. 尝试获取玩家身上的生命脚本
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            // 3. 如果找到了，就让他死
            if (playerHealth != null)
            {
                playerHealth.Die();
            }
        }
    }
}