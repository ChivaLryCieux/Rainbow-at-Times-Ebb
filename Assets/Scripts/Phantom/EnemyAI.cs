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
    private Rigidbody rb; // 缓存 Rigidbody，提高性能

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); // 在 Start 中获取，避免每次调用 GetComponent

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
        
        isDead = true; // 1. 先标记死亡，防止 Update 继续运行逻辑
        anim.SetTrigger("Die");

        // 2. 禁用碰撞体
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        
        // 3. 处理刚体（修复报错的核心）
        if (rb != null) 
        {
            // 关键步骤：先强制把速度归零！
            rb.velocity = Vector3.zero; 
            rb.angularVelocity = Vector3.zero; 
            
            // 然后再开启 Kinematic，接管物理控制权
            rb.isKinematic = true;
        }

        // 【可选建议】如果你还有其他的移动脚本（例如 EnemyMovement），最好在这里禁用它
        // var movementScript = GetComponent<YourMovementScript>();
        // if (movementScript != null) movementScript.enabled = false;
        
        // 【可选建议】如果有 NavMeshAgent，也需要停止
        // var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        // if (agent != null) agent.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawWireSphere(transform.position, killRange);

        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawWireSphere(transform.position, deathRange);
    }
}