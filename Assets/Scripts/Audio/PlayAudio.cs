using UnityEngine;

public class OneTimeMusicTrigger : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("确保你的玩家物体拥有这个Tag")]
    public string playerTag = "Player"; // 玩家的标签

    private AudioSource audioSource;
    private bool hasPlayed = false; // 核心：记录是否已经播放过

    void Start()
    {
        // 获取当前物体上的 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.LogError("物体缺少 AudioSource 组件！请在 Inspector 中添加。");
        }
    }

    // 当有物体进入触发区域时调用
    void OnTriggerEnter(Collider other)
    {
        // 1. 检查是否已经播放过 (hasPlayed 为 false)
        // 2. 检查进入区域的是否是玩家 (通过 Tag 判断)
        if (!hasPlayed && other.CompareTag(playerTag))
        {
            PlayMusic();
        }
    }

    void PlayMusic()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            hasPlayed = true; // 锁定状态，下次再踩也不会播放
            Debug.Log("音乐已触发，且不会再次播放。");
        }
    }
}