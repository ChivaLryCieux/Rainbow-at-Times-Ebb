using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioOnTrigger : MonoBehaviour
{
    [Header("音频设置")]
    [SerializeField] private AudioClip audioToPlay;  // 要播放的音频
    [SerializeField] private bool playOnce = true;   // 是否只播放一次
    
    [Header("触发设置")]
    [SerializeField] private string targetTag = "Player";  // 指定物体的标签
    [SerializeField] private float volume = 1.0f;            // 音量
    
    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // 获取或添加 AudioSource 组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // 设置 AudioSource
        audioSource.clip = audioToPlay;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否碰撞到指定标签的物体
        if (other.CompareTag(targetTag))
        {
            PlayAudio();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 2D版本
        if (other.CompareTag(targetTag))
        {
            PlayAudio();
        }
    }

    private void PlayAudio()
    {
        // 如果设置只播放一次且已经播放过，则返回
        if (playOnce && hasPlayed)
        {
            return;
        }
        
        // 播放音频
        if (audioToPlay != null && audioSource != null)
        {
            audioSource.Play();
            hasPlayed = true;
        }
        else
        {
            Debug.LogWarning("音频或AudioSource未设置！");
        }
    }
    
    // 可选：重置播放状态的方法
    public void ResetAudio()
    {
        hasPlayed = false;
    }
    
    // 可选：设置新的音频
    public void SetAudio(AudioClip newAudio)
    {
        audioToPlay = newAudio;
        audioSource.clip = newAudio;
    }
}