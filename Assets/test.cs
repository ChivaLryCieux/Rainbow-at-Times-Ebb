using System;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Header("Music")] 
    public AudioSource audioSource;
    public bool isPlayOnce = true;
    private bool hasPlayed = false;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasPlayed)
        {
            PlayTargetAudio();
        }
    }

    private void PlayTargetAudio()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            if (isPlayOnce)
            {
                hasPlayed = true;
            }
            Destroy(this, audioSource.clip.length);
        }
        else
        {
            Debug.Log("Error");
        }
    }
    
    
}

