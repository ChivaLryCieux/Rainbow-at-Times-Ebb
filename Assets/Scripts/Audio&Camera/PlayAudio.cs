using UnityEngine;
using System.Collections;
using TMPro; 

public class DynamicMusicTrigger : MonoBehaviour
{
    [Header("通用设置")]
    public string playerTag = "Player";
    
    [Header("文本组件")]
    [Tooltip("场景里那个显示字幕的 TextMeshPro 组件")]
    public TextMeshProUGUI subtitleTextComponent; 
    
    [Header("旁白音频")]
    [Tooltip("进入这个区域要播放的音频")]
    public AudioClip myAudioClip; 
    
    [Tooltip("旁白字幕")]
    [TextArea(3, 10)] // 让输入框变大，方便写长句子
    public string mySubtitleContent; 

    [Tooltip("这条字幕显示多久(秒)")]
    public float duration = 5.0f;

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // 获取自身的 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
            audioSource = gameObject.AddComponent<AudioSource>(); // 如果忘了加，自动加一个

        // 这里的 AudioSource 不需要预先赋值 AudioClip，我们会动态赋值
        audioSource.playOnAwake = false;
        
        // 游戏开始时确保文字是空的
        if(subtitleTextComponent != null)
            subtitleTextComponent.text = "";
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hasPlayed && other.CompareTag(playerTag))
        {
            PlayContent();
        }
    }

    void PlayContent()
    {
        // 1. 处理音频
        if (myAudioClip != null)
        {
            audioSource.clip = myAudioClip; // 换碟
            audioSource.Play();             // 播放
        }

        // 2. 处理字幕
        if (subtitleTextComponent != null)
        {
            // 停止之前可能正在运行的隐藏倒计时（防止上一句还没说完就被隐藏了）
            StopAllCoroutines(); 
            
            // 修改文字内容
            subtitleTextComponent.text = mySubtitleContent;
            
            // 开启新的倒计时
            StartCoroutine(ClearSubtitleRoutine());
        }

        hasPlayed = true; // 锁定，只触发一次
    }

    IEnumerator ClearSubtitleRoutine()
    {
        yield return new WaitForSeconds(duration);
        
        // 时间到，清空文字
        if (subtitleTextComponent != null)
        {
            subtitleTextComponent.text = "";
        }
    }
}