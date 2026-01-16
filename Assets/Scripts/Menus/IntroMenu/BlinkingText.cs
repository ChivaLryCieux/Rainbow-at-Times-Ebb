using UnityEngine;
using TMPro;

public class BlinkingText : MonoBehaviour
{
    private TMP_Text m_TextComponent;
    
    [Header("设置")]
    [Tooltip("闪烁速度")]
    public float speed = 2.0f; 
    [Tooltip("最小透明度 (0-1)")]
    public float minAlpha = 0.1f; 
    [Tooltip("最大透明度 (0-1)")]
    public float maxAlpha = 1.0f;

    void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // 1. 计算 Alpha 值
        // Mathf.Sin 返回 -1 到 1 之间的值
        // 我们将其映射到 0 到 1 之间，以便用于 Alpha
        float alpha = (Mathf.Sin(Time.time * speed) + 1.0f) / 2.0f;

        // 2. 根据最小和最大透明度进行插值
        alpha = Mathf.Lerp(minAlpha, maxAlpha, alpha);

        // 3. 应用颜色
        // 注意：我们只修改 alpha 值，保留原有的 RGB
        Color currentColor = m_TextComponent.color;
        currentColor.a = alpha;
        m_TextComponent.color = currentColor;
    }
}