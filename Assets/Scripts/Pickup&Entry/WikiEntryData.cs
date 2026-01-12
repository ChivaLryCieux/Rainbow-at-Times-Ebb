using UnityEngine;

[CreateAssetMenu(fileName = "NewWikiEntry", menuName = "Wiki/Entry")]
public class WikiEntryData : ScriptableObject
{
    public string id;           // 唯一ID，用于解锁判断 (例如: "research_01")
    public string title;        // 标题 (例如: "外星遗迹")
    [TextArea(5, 10)]
    public string description;  // 详细内容
    public Category category;   // 属于哪个分页

    public enum Category
    {
        Research, // 研究
        Archive,  // 档案
        Event     // 事件
    }
}