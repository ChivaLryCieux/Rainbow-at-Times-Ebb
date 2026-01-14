using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // 必须引用
using TMPro; // 必须引用

public class TopTabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("组件")]
    public Image bgImage;          // 背景图片
    public TextMeshProUGUI btnText;// 按钮文字

    [Header("颜色设置")]
    // 未选中时的颜色 (黑底白字)
    public Color normalBgColor = new Color(0f, 0f, 0f, 0.5f); 
    public Color normalTextColor = Color.white;

    // 选中或悬停时的颜色 (白底黑字)
    public Color activeBgColor = Color.white;
    public Color activeTextColor = Color.black;

    private bool isSelected = false; // 当前是否被选中

    // --- 由 Master 调用，用来设置“你是否被选中了” ---
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisuals(isSelected); // 选中了就显示高亮，没选中就显示默认
    }

    // --- 鼠标移入：如果没有被选中，就临时变亮 ---
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
        {
            UpdateVisuals(true); // 临时高亮
        }
    }

    // --- 鼠标移出：如果没有被选中，就恢复默认 ---
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            UpdateVisuals(false); // 恢复默认
        }
    }

    // --- 内部方法：统一更新颜色 ---
    private void UpdateVisuals(bool isActiveState)
    {
        if (bgImage != null) 
            bgImage.color = isActiveState ? activeBgColor : normalBgColor;
        
        if (btnText != null) 
            btnText.color = isActiveState ? activeTextColor : normalTextColor;
    }
}