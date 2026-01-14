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
                GameNotificationUI.Instance.Show($"拾取了编号：{entryIDToUnlock}");
            }

            // 可以在这里播放音效或特效
            
            if (destroyOnPickup)
            {
                Destroy(gameObject); 
            }
        }
    }
}