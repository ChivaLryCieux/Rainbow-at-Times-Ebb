using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class WikiUIItem : MonoBehaviour
{
    [Header("UI 组件设置")]
    public TextMeshProUGUI titleText;      // 用于显示标题
    public TextMeshProUGUI descriptionText; // 用于显示正文内容

    [Header("布局选项")]
    public bool autoResize = true; // 是否自动调整高度

    /// <summary>
    /// 初始化条目内容
    /// </summary>
    /// <param name="title">标题字符串</param>
    /// <param name="content">详细介绍字符串</param>
    public void Setup(string title, string content)
    {
        // 1. 设置标题
        if (titleText != null)
        {
            titleText.text = title;
        }

        // 2. 设置内容
        if (descriptionText != null)
        {
            descriptionText.text = content;
        }

        // 3. (可选) 强制刷新布局
        // 当文本内容很长时，Unity 的 ContentSizeFitter 有时需要一帧才能反应过来
        // 这行代码可以强制它立即计算高度，防止文字重叠
        if (autoResize)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}