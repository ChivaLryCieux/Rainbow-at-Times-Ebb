using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(Collider))]
public class FogHeightController : MonoBehaviour
{
    [Header("雾高度设置")]
    [Tooltip("目标雾高度（玩家进入时）")]
    [SerializeField] private float targetFogHeight = 3000f;
    
    [Tooltip("过渡持续时间（秒）")]
    [SerializeField] private float transitionDuration = 2f;
    
    [Header("全局Volume引用")]
    [SerializeField] private Volume globalVolume;
    
    [Header("调试")]
    [SerializeField] private bool showDebugLogs = false;
    
    // 雾组件引用
    private Fog _fogComponent;
    private float _initialFogHeight = 0f;
    private Coroutine _currentFogTransition;
    
    private void Start()
    {
        // 如果没有手动指定Volume，尝试自动查找
        if (globalVolume == null)
        {
            globalVolume = FindObjectOfType<Volume>();
            
            if (globalVolume == null)
            {
                Debug.LogError("未找到全局Volume！请手动将全局Volume拖拽到脚本中。");
                return;
            }
        }
        
        // 确保碰撞体是触发器
        GetComponent<Collider>().isTrigger = true;
        
        // 获取Fog组件
        if (!globalVolume.profile.TryGet(out _fogComponent))
        {
            Debug.LogError("全局Volume中未找到Fog组件！请确保Volume Profile中启用了Fog。");
            return;
        }
        
        // 保存初始雾高度
        _initialFogHeight = _fogComponent.baseHeight.value;
        
        if (showDebugLogs)
        {
            Debug.Log($"FogHeightController初始化完成。初始雾高度: {_initialFogHeight}");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fogComponent == null) return;
            
            if (showDebugLogs)
            {
                Debug.Log($"玩家进入触发器区域，开始升高雾高度到 {targetFogHeight}");
            }
            
            // 停止当前正在运行的过渡协程
            if (_currentFogTransition != null)
            {
                StopCoroutine(_currentFogTransition);
            }
            
            // 开始新的过渡
            _currentFogTransition = StartCoroutine(TransitionFogHeight(targetFogHeight));
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_fogComponent == null) return;
            
            if (showDebugLogs)
            {
                Debug.Log($"玩家离开触发器区域，开始降低雾高度到 {_initialFogHeight}");
            }
            
            // 停止当前正在运行的过渡协程
            if (_currentFogTransition != null)
            {
                StopCoroutine(_currentFogTransition);
            }
            
            // 开始新的过渡
            _currentFogTransition = StartCoroutine(TransitionFogHeight(_initialFogHeight));
        }
    }
    
    private IEnumerator TransitionFogHeight(float targetHeight)
    {
        if (_fogComponent == null) yield break;
        
        float startHeight = _fogComponent.baseHeight.value;
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            
            // 使用平滑的插值
            float newHeight = Mathf.Lerp(startHeight, targetHeight, SmoothStep(t));
            
            // 应用新的雾高度
            _fogComponent.baseHeight.value = newHeight;
            
            if (showDebugLogs && elapsedTime % 0.5f < Time.deltaTime) // 每0.5秒输出一次日志
            {
                Debug.Log($"过渡中: 雾高度 = {newHeight:F2}");
            }
            
            yield return null;
        }
        
        // 确保达到目标值
        _fogComponent.baseHeight.value = targetHeight;
        
        if (showDebugLogs)
        {
            Debug.Log($"雾高度过渡完成: {targetHeight}");
        }
    }
    
    // 平滑过渡函数（可选的缓动函数）
    private float SmoothStep(float t)
    {
        // 使用三次平滑函数
        return t * t * (3f - 2f * t);
        
        // 您也可以尝试其他缓动函数：
        // return Mathf.SmoothStep(0f, 1f, t);
        // return 1f - Mathf.Pow(1f - t, 3f); // 三次缓出
    }
    
    // 在检视面板中快速测试
    [ContextMenu("测试: 升高雾高度")]
    private void TestRaiseFog()
    {
        if (_fogComponent != null)
        {
            if (_currentFogTransition != null)
                StopCoroutine(_currentFogTransition);
            
            _currentFogTransition = StartCoroutine(TransitionFogHeight(targetFogHeight));
        }
    }
    
    [ContextMenu("测试: 降低雾高度")]
    private void TestLowerFog()
    {
        if (_fogComponent != null)
        {
            if (_currentFogTransition != null)
                StopCoroutine(_currentFogTransition);
            
            _currentFogTransition = StartCoroutine(TransitionFogHeight(_initialFogHeight));
        }
    }
    
    // 确保在禁用时重置
    private void OnDisable()
    {
        if (_currentFogTransition != null)
        {
            StopCoroutine(_currentFogTransition);
        }
    }
    
    // 可选：在编辑器中可视化触发器范围
    private void OnDrawGizmos()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;
        
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
    
    private void OnDrawGizmosSelected()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return;
        
        Gizmos.color = new Color(0, 1, 1, 0.5f);
        Gizmos.DrawCube(collider.bounds.center, collider.bounds.size);
    }
}