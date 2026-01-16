using UnityEngine;
using TMPro; // 引用 TextMeshPro
using UnityEngine.EventSystems; // 引用事件系统(必须)

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("目标文字对象")]
    public TMP_Text targetText; // 拖入子物体的 Text 组件

    [Header("颜色设置")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color32(237, 83, 83, 255);

    [Header("缩放设置")]
    public float normalScale = 1.0f;
    public float hoverScale = 1.2f; // 放大 1.2 倍

    [Header("动画平滑度")]
    public float transitionSpeed = 10f;

    // 内部状态变量
    private bool isHovering = false;
    
    // 用于记录初始的 scale (避免以 (1,1,1) 为基准，而是以当前大小为基准)
    private Vector3 initialScaleVector;

    void Start()
    {
        // 如果没有手动拖拽赋值，尝试自动获取子物体的 TMP
        if (targetText == null)
            targetText = GetComponentInChildren<TMP_Text>();

        // 记录一下文字最原始的大小
        if (targetText != null)
            initialScaleVector = targetText.transform.localScale;
        
        // 初始化颜色
        if(targetText != null) 
            targetText.color = normalColor;
    }

    // 当鼠标进入按钮区域
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    // 当鼠标移出按钮区域
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void Update()
    {
        if (targetText == null) return;

        // 1. 确定目标值
        Color targetColor = isHovering ? hoverColor : normalColor;
        float targetScaleMult = isHovering ? hoverScale : normalScale;

        // 2. 平滑过渡颜色 (Lerp)
        targetText.color = Color.Lerp(targetText.color, targetColor, Time.deltaTime * transitionSpeed);

        // 3. 平滑过渡缩放 (Lerp)
        // 我们基于 initialScaleVector 进行乘法，这样无论文字本身多大，都会按比例放大
        Vector3 targetScaleVec = initialScaleVector * targetScaleMult;
        targetText.transform.localScale = Vector3.Lerp(targetText.transform.localScale, targetScaleVec, Time.deltaTime * transitionSpeed);
    }
}