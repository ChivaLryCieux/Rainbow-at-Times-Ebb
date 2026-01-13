using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems; // 1. 必须引入这个命名空间

// 2. 在这里加上接口声明：IPointerEnterHandler, IPointerExitHandler
public class WikiTitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button btn;
    public TextMeshProUGUI titleText;

    // 保存默认颜色和悬停颜色
    private Color normalTextColor = Color.white; // 默认白字
    private Color hoverTextColor = Color.black;  // 悬停黑字

    public void Setup(WikiEntryData data, Action<WikiEntryData> onClickAction)
    {
        titleText.text = data.title;
        
        // 确保一开始文字是白色的
        titleText.color = normalTextColor;

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => 
        {
            onClickAction(data);
        });
    }

    // --- 鼠标移入时触发 ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(titleText != null)
            titleText.color = hoverTextColor; // 变成黑色
    }

    // --- 鼠标移出时触发 ---
    public void OnPointerExit(PointerEventData eventData)
    {
        if(titleText != null)
            titleText.color = normalTextColor; // 变回白色
    }
}