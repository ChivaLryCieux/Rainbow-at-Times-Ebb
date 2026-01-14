using UnityEngine;

public class WikiPickup : MonoBehaviour
{
    public string entryIDToUnlock; 
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WikiManager.Instance.UnlockEntry(entryIDToUnlock);
            
            // 字幕显示
            if (GameNotificationUI.Instance != null)
            {
                GameNotificationUI.Instance.Show($"已读取信息：{entryIDToUnlock} \r\n按下Esc进入研究界面阅读");
            }

            // 可以在这里播放音效或特效
            
            if (destroyOnPickup)
            {
                Destroy(gameObject); 
            }
        }
    }
}