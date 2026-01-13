using UnityEngine;
using System.Collections;

public class CameraZoneTrigger : MonoBehaviour
{
    [Header("相机设置")]
    [Tooltip("主相机（不填则自动查找）")]
    public Camera targetCamera;
    
    [Header("拉远设置")]
    [Tooltip("拉远后的相机偏移量（相对角色的偏移）")]
    public Vector3 zoomedOutOffset = new Vector3(0, 5, -15);
    
    [Tooltip("拉远时相机的FOV（视野角度），设为0表示不改变FOV")]
    [Range(0, 120)]
    public float zoomedOutFOV = 0f;
    
    [Header("平滑过渡设置")]
    [Tooltip("过渡到拉远状态所需时间（秒）")]
    [Range(0.5f, 3f)]
    public float zoomInDuration = 1.5f;
    
    [Tooltip("过渡到正常状态所需时间（秒）")]
    [Range(0.5f, 3f)]
    public float zoomOutDuration = 1f;
    
    [Tooltip("过渡曲线 - 控制过渡的速度变化")]
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("触发设置")]
    [Tooltip("触发器的标签（只跟随指定标签的对象）")]
    public string targetTag = "Player";
    
    [Tooltip("是否在进入时立即触发")]
    public bool triggerOnEnter = true;
    
    [Tooltip("是否在退出时恢复")]
    public bool triggerOnExit = true;
    
    [Header("多玩家支持")]
    [Tooltip("允许多个玩家同时触发")]
    public bool allowMultiplePlayers = false;
    
    [Header("调试信息")]
    [Tooltip("在控制台显示过渡信息")]
    public bool showDebugLogs = false;
    
    [SerializeField]
    private float currentTransitionProgress = 0f;
    
    [SerializeField]
    private bool isZoomedOut = false;
    
    // 原始状态
    private Vector3 originalOffset;
    private float originalFOV;
    private CameraFollow cameraFollow;
    
    // 协程引用
    private Coroutine currentTransitionCoroutine;
    
    // 当前在区域内的玩家数量
    private int playersInZone = 0;
    
    void Start()
    {
        // 自动查找主相机
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        if (targetCamera == null)
        {
            Debug.LogError("未找到主相机！请指定相机或确保场景中有标记为MainCamera的相机。", this);
            return;
        }
        
        // 获取CameraFollow脚本
        cameraFollow = targetCamera.GetComponent<CameraFollow>();
        if (cameraFollow == null)
        {
            Debug.LogError("相机上没有找到CameraFollow脚本！", this);
            return;
        }
        
        // 保存原始状态
        originalOffset = cameraFollow.offset;
        originalFOV = targetCamera.fieldOfView;
        
        // 设置默认过渡曲线（如果未指定）
        if (transitionCurve == null || transitionCurve.keys.Length == 0)
        {
            transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
        
        // 确保Collider是触发器
        Collider collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("CameraZoneTrigger需要挂载在带有Collider的物体上，且Collider应设置为IsTrigger。", this);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!triggerOnEnter) return;
        if (cameraFollow == null) return;
        
        // 检查标签
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;
        
        playersInZone++;
        
        // 如果已经有玩家在区域内且不允许多人，则忽略
        if (playersInZone > 1 && !allowMultiplePlayers) return;
        
        if (showDebugLogs) Debug.Log($"玩家进入相机区域，区域内玩家数: {playersInZone}");
        
        // 如果已经在拉远状态，不需要再次触发
        if (isZoomedOut) return;
        
        // 开始拉远过渡
        StartTransitionToZoomedOut();
    }
    
    void OnTriggerExit(Collider other)
    {
        if (!triggerOnExit) return;
        if (cameraFollow == null) return;
        
        // 检查标签
        if (!string.IsNullOrEmpty(targetTag) && !other.CompareTag(targetTag)) return;
        
        playersInZone--;
        
        // 确保玩家数量不会小于0
        if (playersInZone < 0) playersInZone = 0;
        
        if (showDebugLogs) Debug.Log($"玩家离开相机区域，区域内玩家数: {playersInZone}");
        
        // 如果还有玩家在区域内，则不恢复
        if (playersInZone > 0 && allowMultiplePlayers) return;
        
        // 如果已经不在拉远状态，不需要再次触发
        if (!isZoomedOut) return;
        
        // 开始恢复正常过渡
        StartTransitionToNormal();
    }
    
    void StartTransitionToZoomedOut()
    {
        if (showDebugLogs) Debug.Log("开始平滑过渡到拉远视角");
        
        // 如果已经有过渡在进行，先停止它
        if (currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }
        
        // 开始新的过渡协程
        currentTransitionCoroutine = StartCoroutine(TransitionToZoomedOutCoroutine());
    }
    
    void StartTransitionToNormal()
    {
        if (showDebugLogs) Debug.Log("开始平滑过渡到正常视角");
        
        // 如果已经有过渡在进行，先停止它
        if (currentTransitionCoroutine != null)
        {
            StopCoroutine(currentTransitionCoroutine);
        }
        
        // 开始新的过渡协程
        currentTransitionCoroutine = StartCoroutine(TransitionToNormalCoroutine());
    }
    
    IEnumerator TransitionToZoomedOutCoroutine()
    {
        isZoomedOut = true;
        currentTransitionProgress = 0f;
        
        // 起始值
        Vector3 startOffset = cameraFollow.offset;
        float startFOV = targetCamera.fieldOfView;
        
        // 目标值
        Vector3 targetOffset = zoomedOutOffset;
        float targetFOV = zoomedOutFOV > 0 ? zoomedOutFOV : startFOV;
        
        float time = 0f;
        
        while (time < zoomInDuration)
        {
            time += Time.deltaTime;
            currentTransitionProgress = time / zoomInDuration;
            
            // 使用曲线控制过渡速度
            float curveValue = transitionCurve.Evaluate(currentTransitionProgress);
            
            // 计算插值
            Vector3 newOffset = Vector3.Lerp(startOffset, targetOffset, curveValue);
            float newFOV = Mathf.Lerp(startFOV, targetFOV, curveValue);
            
            // 应用新的值
            cameraFollow.offset = newOffset;
            if (zoomedOutFOV > 0)
            {
                targetCamera.fieldOfView = newFOV;
            }
            
            if (showDebugLogs && time % 0.5f < Time.deltaTime)
            {
                Debug.Log($"拉远过渡进度: {currentTransitionProgress:P0}, 当前偏移: {newOffset}");
            }
            
            yield return null;
        }
        
        // 确保最终到达目标
        cameraFollow.offset = targetOffset;
        if (zoomedOutFOV > 0)
        {
            targetCamera.fieldOfView = targetFOV;
        }
        
        currentTransitionProgress = 1f;
        
        if (showDebugLogs) Debug.Log($"拉远过渡完成，最终偏移: {targetOffset}");
    }
    
    IEnumerator TransitionToNormalCoroutine()
    {
        isZoomedOut = false;
        currentTransitionProgress = 0f;
        
        // 起始值
        Vector3 startOffset = cameraFollow.offset;
        float startFOV = targetCamera.fieldOfView;
        
        // 目标值
        Vector3 targetOffset = originalOffset;
        float targetFOV = originalFOV;
        
        float time = 0f;
        
        while (time < zoomOutDuration)
        {
            time += Time.deltaTime;
            currentTransitionProgress = time / zoomOutDuration;
            
            // 使用曲线控制过渡速度
            float curveValue = transitionCurve.Evaluate(currentTransitionProgress);
            
            // 计算插值
            Vector3 newOffset = Vector3.Lerp(startOffset, targetOffset, curveValue);
            float newFOV = Mathf.Lerp(startFOV, targetFOV, curveValue);
            
            // 应用新的值
            cameraFollow.offset = newOffset;
            if (zoomedOutFOV > 0)
            {
                targetCamera.fieldOfView = newFOV;
            }
            
            if (showDebugLogs && time % 0.5f < Time.deltaTime)
            {
                Debug.Log($"恢复过渡进度: {currentTransitionProgress:P0}, 当前偏移: {newOffset}");
            }
            
            yield return null;
        }
        
        // 确保最终到达目标
        cameraFollow.offset = targetOffset;
        if (zoomedOutFOV > 0)
        {
            targetCamera.fieldOfView = targetFOV;
        }
        
        currentTransitionProgress = 1f;
        
        if (showDebugLogs) Debug.Log($"恢复过渡完成，最终偏移: {targetOffset}");
    }
    
    // 在编辑器模式中，可以手动测试
    [ContextMenu("测试拉远效果")]
    public void TestZoomOut()
    {
        if (cameraFollow != null)
        {
            StartTransitionToZoomedOut();
        }
    }
    
    [ContextMenu("测试恢复效果")]
    public void TestZoomNormal()
    {
        if (cameraFollow != null)
        {
            StartTransitionToNormal();
        }
    }
    
    [ContextMenu("立即拉远")]
    public void ImmediateZoomOut()
    {
        if (cameraFollow != null)
        {
            cameraFollow.offset = zoomedOutOffset;
            if (zoomedOutFOV > 0)
            {
                targetCamera.fieldOfView = zoomedOutFOV;
            }
            isZoomedOut = true;
            if (showDebugLogs) Debug.Log($"立即拉远到: {zoomedOutOffset}");
        }
    }
    
    [ContextMenu("立即恢复")]
    public void ImmediateZoomNormal()
    {
        if (cameraFollow != null)
        {
            cameraFollow.offset = originalOffset;
            targetCamera.fieldOfView = originalFOV;
            isZoomedOut = false;
            if (showDebugLogs) Debug.Log($"立即恢复到: {originalOffset}");
        }
    }
    
    // 在编辑器中可视化触发器范围
    void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;
        
        // 根据状态选择颜色
        Color fillColor = isZoomedOut ? 
            new Color(1f, 0.5f, 0f, 0.3f) : 
            new Color(0f, 1f, 0.5f, 0.3f);
        
        Gizmos.color = fillColor;
        
        if (collider is BoxCollider boxCollider)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);
            Gizmos.color = isZoomedOut ? Color.yellow : Color.cyan;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (collider is SphereCollider sphereCollider)
        {
            Vector3 center = transform.TransformPoint(sphereCollider.center);
            Gizmos.DrawSphere(center, sphereCollider.radius);
            Gizmos.color = isZoomedOut ? Color.yellow : Color.cyan;
            Gizmos.DrawWireSphere(center, sphereCollider.radius);
        }
    }
}