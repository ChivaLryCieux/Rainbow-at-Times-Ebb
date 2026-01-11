using UnityEngine;

public class WikiPickup : MonoBehaviour
{
    public string entryIDToUnlock; // 在Inspector里填入对应的ID，如 "research_01"
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 解锁
            WikiManager.Instance.UnlockEntry(entryIDToUnlock);
            
            // 可以在这里播放音效或特效
            
            if (destroyOnPickup)
            {
                Destroy(gameObject); // 销毁物体
            }
        }
    }
}