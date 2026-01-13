using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    [Header("参数设置")]
    public float fallDelay = 1.0f;      // 站上去多久后坠落
    public float destroyDelay = 3.0f;   // 坠落多久后销毁
    public float shakeIntensity = 0.05f;// 抖动幅度

    private Rigidbody rb;
    private bool isTriggered = false;
    private Vector3 initialPosition;
    private Renderer myRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
        initialPosition = transform.position;

        // 初始状态：不受重力，悬在空中
        rb.isKinematic = true;
    }

    // --- 修改点：改用 Trigger 检测 ---
    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        // CharacterController 必定带有 Collider，所以这里能检测到
        if (other.CompareTag("Player") || other.CompareTag("PhantomPlayer"))
        {
            StartCoroutine(FallRoutine());
        }
    }

    IEnumerator FallRoutine()
    {
        isTriggered = true;

        float timer = 0f;
        while (timer < fallDelay)
        {
            timer += Time.deltaTime;

            // 抖动效果
            if (shakeIntensity > 0)
            {
                transform.position = initialPosition + Random.insideUnitSphere * shakeIntensity;
            }

            // 变红警告
            if (myRenderer != null)
            {
                myRenderer.material.color = Color.Lerp(Color.white, Color.red, timer / fallDelay);
            }

            yield return null;
        }

        // 下坠开始
        transform.position = initialPosition; // 修正抖动位移
        rb.isKinematic = false; // 关闭运动学，开启物理模拟
        rb.useGravity = true;   // 开启重力

        Destroy(gameObject, destroyDelay);
    }
}