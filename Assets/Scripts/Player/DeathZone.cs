using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // 必须引入这个

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Die();
                // 开启不受时间暂停影响的协程
                StartCoroutine(LoadOverMenuWithDelay());
            }
        }
    }

    IEnumerator LoadOverMenuWithDelay()
    {
        // 使用 Realtime 等待，这样即使 Time.timeScale = 0 也能继续走
        yield return new WaitForSecondsRealtime(1f); 

        SceneManager.LoadScene("OverMenu");
    }
}