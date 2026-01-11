using System.Collections.Generic;
using UnityEngine;

public class WikiManager : MonoBehaviour
{
    public static WikiManager Instance;

    // 存储已解锁的词条ID (使用HashSet防止重复，且查找快)
    private HashSet<string> unlockedEntryIDs = new HashSet<string>();

    // 所有的词条数据引用 (需要在Inspector里把所有做好的ScriptableObject拖进去)
    public List<WikiEntryData> allEntries; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject); // 切换场景时不销毁
    }

    // 解锁一个词条
    public void UnlockEntry(string entryID)
    {
        if (!unlockedEntryIDs.Contains(entryID))
        {
            unlockedEntryIDs.Add(entryID);
            Debug.Log($"词条 {entryID} 已解锁！");
            // 这里可以加一个UI弹窗提示 "获得新档案"
        }
    }

    // 检查是否解锁
    public bool IsUnlocked(string entryID)
    {
        return unlockedEntryIDs.Contains(entryID);
    }

    // 获取某个分类下所有已解锁的词条
    public List<WikiEntryData> GetUnlockedEntriesByCategory(WikiEntryData.Category category)
    {
        List<WikiEntryData> result = new List<WikiEntryData>();
        foreach (var entry in allEntries)
        {
            if (entry.category == category && IsUnlocked(entry.id))
            {
                result.Add(entry);
            }
        }
        return result;
    }
}