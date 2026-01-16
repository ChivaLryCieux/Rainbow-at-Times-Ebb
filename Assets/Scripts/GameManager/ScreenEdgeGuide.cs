using UnityEngine;
using UnityEngine.UI;

public class ScreenEdgeGuide : MonoBehaviour
{
    [Header("外观设置")]
    public float padding = 40f;          // 离屏幕边缘的距离
    public float iconScale = 1.0f;       // 图标基础大小
    public float fadeDistance = 3.0f;    // 离目标多近时消失

    [Header("呼吸效果")]
    public bool enableBreathing = true;
    public float breatheSpeed = 2.0f;    // 呼吸速度
    public float minAlpha = 0.3f;        // 最暗透明度 (0-1)
    public float maxAlpha = 0.8f;        // 最亮透明度 (0-1)

    private RectTransform rectTransform;
    private Image targetImage;
    private Camera mainCam;
    private CanvasGroup canvasGroup;     // 用于控制整体透明度

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        targetImage = GetComponent<Image>();
        mainCam = Camera.main;

        // 添加 CanvasGroup 组件方便控制淡入淡出
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Update()
    {
        // 1. 获取目标位置
        Vector3? targetPos = GameManager.Instance.GetNextCheckpointPosition();

        // 如果没有目标，或者离目标太近，隐藏
        if (targetPos == null || Vector3.Distance(mainCam.transform.position, targetPos.Value) < fadeDistance)
        {
            canvasGroup.alpha = 0f; // 柔和隐藏
            return;
        }

        // 2. 将 3D 坐标转换为屏幕坐标
        Vector3 screenPos = mainCam.WorldToScreenPoint(targetPos.Value);

        // 3. 处理目标在相机背后的情况 (z < 0)
        // 如果点在背后，坐标会翻转，需要修正
        if (screenPos.z < 0)
        {
            screenPos.x = Screen.width - screenPos.x;
            screenPos.y = Screen.height - screenPos.y;
        }

        // 4. 计算屏幕中心和边界
        Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2, 0);

        // 这里的 min/max 是光点活动的“安全框”
        float minX = padding;
        float maxX = Screen.width - padding;
        float minY = padding;
        float maxY = Screen.height - padding;

        // 5. 限制坐标在屏幕边缘 (核心逻辑)
        // 我们不只是 Clamp，而是要让它看起来像是指向那个方向
        screenPos.x = Mathf.Clamp(screenPos.x, minX, maxX);
        screenPos.y = Mathf.Clamp(screenPos.y, minY, maxY);

        // 应用位置
        rectTransform.position = Vector3.Lerp(rectTransform.position, screenPos, Time.deltaTime * 10f); // 增加一点平滑移动

        // 6. 呼吸效果 (让它像是有生命的能量)
        if (enableBreathing)
        {
            float wave = (Mathf.Sin(Time.time * breatheSpeed) + 1f) / 2f; // 0 到 1 的波形

            // 大小呼吸
            float currentScale = iconScale * (0.8f + wave * 0.4f);
            rectTransform.localScale = Vector3.one * currentScale;

            // 透明度呼吸
            canvasGroup.alpha = Mathf.Lerp(minAlpha, maxAlpha, wave);
        }
        else
        {
            canvasGroup.alpha = maxAlpha;
        }
    }
}