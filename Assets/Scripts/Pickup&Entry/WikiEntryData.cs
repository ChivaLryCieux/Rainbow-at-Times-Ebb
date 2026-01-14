using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "NewWikiEntry", menuName = "Wiki/Entry")]
public class WikiEntryData : ScriptableObject
{
    [Header("Basic Information")]
    public string id;
    public string title;
    
    [TextArea(5, 10)]
    public string description;
    
    public Category category;

    // --- 媒体区域 ---
    [Header("Media Content (Optional)")]
    
    [Tooltip("主图：通常用于列表展示。如果不填，UI应隐藏相关图片组件。")]
    [FormerlySerializedAs("entryImage")] 
    public Sprite primaryImage; 

    [Tooltip("副图：详情页的补充图片，可为空。")]
    public Sprite secondaryImage; 

    [Tooltip("视频：可为空。支持 .mp4, .mov 等 Unity 支持的格式。")]
    public VideoClip wikiVideo; 

    public enum Category
    {
        Research,
        Event
    }
    
    // 这是一个辅助方法，方便你在代码中快速检查是否有视频
    public bool HasVideo()
    {
        return wikiVideo != null;
    }
}