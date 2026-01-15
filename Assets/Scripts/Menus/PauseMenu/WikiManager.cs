using System.Collections.Generic;
using UnityEngine;

public class WikiManager : MonoBehaviour
{
    public static WikiManager Instance;

    // 存储已解锁的词条ID
    private HashSet<string> unlockedEntryIDs = new HashSet<string>();

    // 所有的词条数据引用
    public List<WikiEntryData> allEntries; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject); 
    }
    
    public bool UnlockEntry(string entryID)
    {
        // 如果是新ID，Add返回true；如果是旧ID，Add返回false
        if (unlockedEntryIDs.Add(entryID))
        {
            Debug.Log($"词条 {entryID} 首次解锁！");
            return true; 
        }
        return false;
    }

    // 根据ID查找数据对象
    public WikiEntryData GetEntryByID(string id)
    {
        foreach (var entry in allEntries)
        {
            if (entry.id == id) return entry;
        }
        return null;
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