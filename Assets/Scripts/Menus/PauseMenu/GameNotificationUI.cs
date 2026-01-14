using UnityEngine;
using TMPro;
using System.Collections;

public class GameNotificationUI : MonoBehaviour
{
    public static GameNotificationUI Instance { get; private set; }

    [Header("UI 组件")]
    public TextMeshProUGUI notificationText;
    public CanvasGroup canvasGroup;

    [Header("设置")]
    public float displayDuration = 2.0f; // 显示多久
    public float fadeSpeed = 2.0f;       // 淡出速度

    private Coroutine currentCoroutine;

    void Awake()
    {
        // 单例设置
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 初始状态：完全透明（隐藏）
        if (canvasGroup != null) canvasGroup.alpha = 0f;
    }

    // 外部调用的方法
    public void Show(string message)
    {
        if (notificationText != null) notificationText.text = message;

        // 如果上一个提示还在显示，先停止它，重新开始
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(FadeRoutine());
    }

    IEnumerator FadeRoutine()
    {
        // 1. 瞬间显示 (Alpha = 1)
        if (canvasGroup != null) canvasGroup.alpha = 1f;

        // 2. 保持显示一段时间
        yield return new WaitForSeconds(displayDuration);

        // 3. 慢慢淡出
        while (canvasGroup != null && canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}