using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class NPCChasePlayer : MonoBehaviour
{
    [Header("追击设置")]
    public float chaseRange = 10f;
    public float moveSpeed = 3f;
    public float stopDistance = 1.5f; 
    public float detectSneakRange = 2f;
    public float rotateSpeed = 10f; 

    [Header("引用")]
    public Transform player;
    private PlayerSneak playerSneak; 
    private Rigidbody rb;
    private Animator anim;
    
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>(); 
        rb.freezeRotation = true; 

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerSneak = playerObj.GetComponent<PlayerSneak>();
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        // 【新增 1】如果刚体已经不受物理控制（说明在 EnemyAI 里死了），这里也不要跑逻辑了
        if (rb.isKinematic) return;

        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        
        // 注意：这里删掉了 "rb.velocity = Vector3.zero"，
        // 因为 Update 不建议处理物理，且如果此时 Kinematic 为 true 会报错。
        if (currentState.IsName("Killed") || currentState.IsName("Killing"))
        {
            isChasing = false;
            return;
        }

        // 距离判断逻辑保持不变
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isSneaking = (playerSneak != null && playerSneak.isSneaking);

        if (isSneaking)
        {
            isChasing = distanceToPlayer <= detectSneakRange && distanceToPlayer > stopDistance;
        }
        else
        {
            isChasing = distanceToPlayer <= chaseRange && distanceToPlayer > stopDistance;
        }
    }

    void FixedUpdate()
    {
        // 【新增 2 - 核心修复】 
        // 如果 EnemyAI 已经把 isKinematic 设为 true，
        // 说明角色死亡，绝对不能再碰 velocity，直接退出！
        if (rb.isKinematic) return;

        // 下面的逻辑只有在 "活着" (isKinematic == false) 的时候才会执行
        AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
        
        // 即使是这里想要刹车，也必须保证 isKinematic 是 false
        if (currentState.IsName("Killed") || currentState.IsName("Killing"))
        {
            rb.velocity = Vector3.zero; 
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
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; 
        
        rb.velocity = new Vector3(direction.x * moveSpeed, rb.velocity.y, direction.z * moveSpeed);

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotateSpeed * Time.deltaTime);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectSneakRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}