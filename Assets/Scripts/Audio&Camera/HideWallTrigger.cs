using UnityEngine;

public class HideWallTrigger : MonoBehaviour
{
    [Tooltip("需要隐藏的墙壁对象")]
    public GameObject wallToHide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && wallToHide != null)
        {
            // 隐藏墙壁 (只关渲染，不关碰撞，这样主角不会掉出去)
            if(wallToHide.GetComponent<Renderer>() != null)
                wallToHide.GetComponent<Renderer>().enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && wallToHide != null)
        {
            // 恢复显示
            if(wallToHide.GetComponent<Renderer>() != null)
                wallToHide.GetComponent<Renderer>().enabled = true;
        }
    }
}
