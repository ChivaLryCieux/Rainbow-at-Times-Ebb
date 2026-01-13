using UnityEngine;

[CreateAssetMenu(fileName = "NewWikiEntry", menuName = "Wiki/Entry")]
public class WikiEntryData : ScriptableObject
{
    public string id;
    public string title;
    [TextArea(5, 10)]
    public string description;
    
    public Sprite entryImage; // 用于存放插图
    // ------------------

    public Category category;
    
    public enum Category
    {
        Research,
        Archive,
        Event
    }
}