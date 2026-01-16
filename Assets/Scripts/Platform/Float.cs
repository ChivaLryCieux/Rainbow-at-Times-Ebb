using UnityEngine;

public class FloatingEffect : MonoBehaviour
{
    [Header("浮动设置")]
    [Tooltip("浮动幅度（米）")]
    [Range(0.01f, 10f)]
    public float floatStrength = 1f;
    
    [Tooltip("浮动速度")]
    [Range(0.5f, 3f)]
    public float floatSpeed = 1f;
    
    [Tooltip("是否在X轴也添加轻微浮动")]
    public bool addHorizontalFloat = true;
    
    [Tooltip("X轴浮动幅度")]
    [Range(0.01f, 10f)]
    public float horizontalStrength = 1f;
    
    [Tooltip("是否旋转")]
    public bool rotate = true;
    
    [Tooltip("旋转速度（度/秒）")]
    [Range(5f, 30f)]
    public float rotationSpeed = 15f;
    
    private Vector3 startPosition;
    private float randomOffset;
    
    void Start()
    {
        // 保存初始位置
        startPosition = transform.position;
        
        // 添加随机偏移，让不同物体的浮动不同步
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }
    
    void Update()
    {
        // 计算基于时间的浮动值
        float time = Time.time * floatSpeed + randomOffset;
        
        // 计算Y轴位移（使用正弦波实现平滑循环）
        float yOffset = Mathf.Sin(time) * floatStrength;
        
        // 计算X轴位移（可选）
        float xOffset = 0f;
        if (addHorizontalFloat)
        {
            xOffset = Mathf.Cos(time * 0.7f) * horizontalStrength;
        }
        
        // 应用位移
        Vector3 newPosition = new Vector3(
            startPosition.x + xOffset,
            startPosition.y + yOffset,
            startPosition.z
        );
        
        transform.position = newPosition;
        
        // 处理旋转（可选）
        if (rotate)
        {
            float rotationAmount = Mathf.Sin(time * 0.5f) * rotationSpeed;
            transform.Rotate(0, rotationAmount * Time.deltaTime, 0);
        }
    }
    
    // 在编辑器中绘制参考线（可选，帮助可视化浮动范围）
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        
        Gizmos.color = Color.cyan;
        Vector3 pos = transform.position;
        
        // 绘制浮动范围指示线
        Gizmos.DrawLine(
            new Vector3(pos.x, pos.y - floatStrength, pos.z),
            new Vector3(pos.x, pos.y + floatStrength, pos.z)
        );
        
        // 绘制起点标记
        Gizmos.DrawWireSphere(pos, 0.02f);
        
        if (addHorizontalFloat)
        {
            Gizmos.DrawLine(
                new Vector3(pos.x - horizontalStrength, pos.y, pos.z),
                new Vector3(pos.x + horizontalStrength, pos.y, pos.z)
            );
        }
    }
}