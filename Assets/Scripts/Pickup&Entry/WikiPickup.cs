using UnityEngine;

public class WikiPickup : MonoBehaviour
{
    public string entryIDToUnlock; 
    public bool destroyOnPickup = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. 尝试解锁，获取是否是第一次
            bool isFirstTime = WikiManager.Instance.UnlockEntry(entryIDToUnlock);
            
            bool shouldAutoOpen = false;

            if (isFirstTime)
            {
                // 2. 如果是第一次，检查是否符合自动播放的条件
                // 条件：分类必须是 Event 且 必须包含视频
                WikiEntryData data = WikiManager.Instance.GetEntryByID(entryIDToUnlock);
                if (data != null && data.category == WikiEntryData.Category.Event && data.HasVideo())
                {
                    shouldAutoOpen = true;
                }
            }

            // 3. 执行逻辑分支
            if (shouldAutoOpen)
            {
                // 情况A：首次拾取Event视频 -> 暂停游戏，隐藏HUD，打开菜单，播放视频
                if (PauseMenuMaster.Instance != null)
                {
                    PauseMenuMaster.Instance.OpenAndPlayVideo(entryIDToUnlock);
                }
            }
            else
            {
                // 情况B：Research条目 或 重复拾取 -> 仅右下角提示，游戏继续
                if (GameNotificationUI.Instance != null)
                {
                    string msg = isFirstTime 
                        ? $"新档案已录入：{entryIDToUnlock}" 
                        : $"已读取信息：{entryIDToUnlock}";
                        
                    GameNotificationUI.Instance.Show($"{msg} \r\n按下Esc进入研究界面阅读");
                }
            }

            // 4. 销毁掉落物
            if (destroyOnPickup)
            {
                Destroy(gameObject); 
            }
        }
    }
}