using UnityEngine;

public class TrapTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        TrapActivator,  // 陷阱触发器（主角踩的）
        TrapReleaser    // 解除开关（幽灵踩的）
    }

    public TriggerType type;
    public PhantomTrapManager manager; // 引用上面的管理器

    private bool hasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        // 情况A：主角踩到了陷阱
        if (type == TriggerType.TrapActivator && other.CompareTag("Player"))
        {
            manager.ActivateTrap();
            hasTriggered = true; // 陷阱是一次性的
        }

        // 情况B：幽灵碰到了开关
        else if (type == TriggerType.TrapReleaser && (other.name == "PhantomPlayer" || other.CompareTag("Phantom")))
        {
            manager.DeactivateTrap();
            hasTriggered = true;
        }
    }
}