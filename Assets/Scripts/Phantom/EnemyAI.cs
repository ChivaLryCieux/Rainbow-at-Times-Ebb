using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("调试开关")]
    public bool showDebugLogs = true; 

    [Header("目标绑定")]
    public Transform playerTarget; 
    public Transform cubeTarget;   

    [Header("距离阈值")]
    public float killRange = 2.0f; 
    public float deathRange = 2.0f;

    private Animator anim;
    private bool isDead = false;   
    private Rigidbody rb; 

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTarget = playerObj.transform;
        }

        if (showDebugLogs)
        {
            Debug.Log($"[EnemyAI] 初始化完成. Player: {playerTarget}, Cube: {cubeTarget}");
            if (anim == null) Debug.LogError("[EnemyAI] 致命错误：没有找到 Animator 组件！");
        }
    }

    void Update()
    {
        if (isDead) return;
        if (playerTarget == null || cubeTarget == null) return;

        float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        float distToCube = Vector3.Distance(transform.position, cubeTarget.position);

        if (showDebugLogs)
        {
            if (distToPlayer <= killRange + 0.5f) 
                Debug.Log($"[距离检测] 离玩家: {distToPlayer:F2} (阈值: {killRange}) | 离Cube: {distToCube:F2}");
        }

        if (distToCube <= deathRange)
        {
            if (showDebugLogs) Debug.LogWarning(">>> 满足死亡条件 (靠近 Cube)");
            DieLogic();
        }
        else if (distToPlayer <= killRange)
        {
            AttackLogic();
        }
    }

    void AttackLogic()
    {
        if (anim.IsInTransition(0)) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Killing")) return; 

        if (showDebugLogs) Debug.Log(">>> 确认触发攻击 Trigger");
        anim.SetTrigger("Attack");
    }

    // --- 修改重点在这里 ---
    void DieLogic()
    {
        if (showDebugLogs) Debug.Log("[触发死亡] 发送 Die Trigger 并锁定状态");
        
        isDead = true; // 1. 先标记死亡
        anim.SetTrigger("Die");

        // 2. 禁用碰撞体
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        // 3. 处理刚体
        if (rb != null) 
        {
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            rb.isKinematic = true;
        }

        // ---【新增功能】---
        // 4. 将物体缩放瞬间变为 (0, 0, 0)
        transform.localScale = Vector3.zero;
        
        // 注意：如果你希望它是“慢慢缩小”而不是“瞬间消失”，
        // 则需要用协程 (Coroutine) 来实现，目前的写法是瞬间消失。
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, killRange);

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, deathRange);
    }
}