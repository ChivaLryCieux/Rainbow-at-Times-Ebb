using UnityEngine;
using TMPro;
using UnityEngine.UI; // 必须引入这个才能控制 Image

public class WikiUIItem : MonoBehaviour
{
    [Header("UI 组件")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    
    // --- 新增 ---
    public Image contentImage; // 用来显示插图的 UI 组件
    // -----------

    public bool autoResize = true;

    // 修改 Setup 方法，增加 sprite 参数
    public void Setup(string title, string content, Sprite sprite)
    {
        if (titleText != null) titleText.text = title;
        if (descriptionText != null) descriptionText.text = content;

        // --- 图片处理逻辑 ---
        if (contentImage != null)
        {
            if (sprite != null)
            {
                // 如果有图：显示图片组件，并赋值
                contentImage.gameObject.SetActive(true);
                contentImage.sprite = sprite;
                
                // 推荐：保持图片比例，防止被拉伸变形
                contentImage.preserveAspect = true; 
            }
            else
            {
                // 如果没图：隐藏图片组件，节省空间
                contentImage.gameObject.SetActive(false);
            }
        }
        // ------------------

        if (autoResize)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}