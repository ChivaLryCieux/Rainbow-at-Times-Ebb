using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPCChasePlayer : MonoBehaviour
{
    [Header("追击设置")]
    public float chaseRange = 10f;       // 正常侦测范围
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f; 
    public float rotateSpeed = 10f; 

    [Header("引用")]
    public Transform player;
    private Rigidbody rb;
    private Animator anim;
    
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); 
        
        // 确保 NPC 不会因为碰撞乱滚
        rb.freezeRotation = true; 

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // 如果刚体已经不受物理控制（例如死亡或被脚本接管），跳过逻辑
        if (rb.isKinematic) return;

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        
        // 如果处于“被杀”或“正在杀人”的动画状态，停止追击
        if (currentState.IsName("Killed") || currentState.IsName("Killing"))
        {
            isChasing = false;
            return;
        }

        // 距离判断逻辑：简化为单一距离检测
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // 只有在范围内且大于停止距离时才进入追击状态
        isChasing = distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance;
    }

    void FixedUpdate()
    {
        // 死亡保护
        if (rb.isKinematic) return;

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        
        // 如果正在播放死亡动画，强行将水平速度归零
        if (currentState.IsName("Killed") || currentState.IsName("Killing"))
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            return;
        }

        if (isChasing && player != null)
        {
            ChasePlayer();
        }
        else
        {
            // 停止时保留垂直速度（重力），水平速度归零
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void ChasePlayer()
    {
        // 计算水平方向
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        
        // 物理移动：保持 Y 轴速度以允许重力作用
        rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);

        // 平滑转向
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        // 绘制检测范围线框
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}