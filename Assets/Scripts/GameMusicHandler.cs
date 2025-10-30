using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GameMusicHandler : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;
    private int lastIndex = -1;
    private int currentIndex = 0;
    private bool isPaused = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayNextClip();
    }

    void Update()
    {
        if (!audioSource.isPlaying && !isPaused) PlayNextClip();
    }

    void PlayNextClip()
    {
        if (audioClips.Length == 0) return;

        int index;
        if (BazookaManager.Instance.GetSettingRandomMusic())
        {
            do index = Random.Range(0, audioClips.Length);
            while (audioClips.Length > 1 && index == lastIndex);
        }
        else
        {
            index = currentIndex;
            currentIndex = (currentIndex + 1) % audioClips.Length;
        }

        lastIndex = index;
        audioSource.clip = audioClips[index];
        audioSource.Play();
    }

    public void PauseMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (isPaused)
        {
            audioSource.Play();
            isPaused = false;
        }
    }

    public void RestartMusic()
    {
        audioSource.Stop();
        audioSource.Play();
    }
}
