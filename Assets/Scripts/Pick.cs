using UnityEngine;
using UnityEngine.Video;

public class SimpleVideoTrigger : MonoBehaviour
{
    [Header("基本设置")]
    public VideoPlayer videoPlayer;  // 拖入VideoPlayer组件
    public string playerTag = "Player";  // 触发物体的标签

    [Header("调试信息")]
    public bool isTriggered = false;

    private void Start()
    {
        Debug.Log("Video Trigger 脚本已启动");
        
        // 自动查找VideoPlayer
        if (videoPlayer == null)
        {
            videoPlayer = FindObjectOfType<VideoPlayer>();
            if (videoPlayer != null)
            {
                Debug.Log("自动找到了VideoPlayer: " + videoPlayer.gameObject.name);
            }
            else
            {
                Debug.LogError("没有找到VideoPlayer组件！请拖拽一个VideoPlayer到脚本上");
            }
        }
    }

    // 使用触发器
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("触发器触发，碰撞对象: " + other.gameObject.name);
        
        if (other.CompareTag(playerTag))
        {
            Debug.Log("检测到玩家标签，开始播放视频");
            PlayVideo();
        }
    }

    // 使用碰撞检测
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("碰撞体触发，碰撞对象: " + collision.gameObject.name);
        
        if (collision.gameObject.CompareTag(playerTag))
        {
            Debug.Log("检测到玩家标签，开始播放视频");
            PlayVideo();
        }
    }

    private void PlayVideo()
    {
        if (isTriggered) return;
        
        if (videoPlayer != null)
        {
            videoPlayer.Play();
            isTriggered = true;
            Debug.Log("视频开始播放！");
        }
        else
        {
            Debug.LogError("VideoPlayer为空！");
        }
    }
}