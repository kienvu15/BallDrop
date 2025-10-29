using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource audioSource;
    public bool sound = true;

    private void Awake()
    {
        MakeSinglition();
        audioSource = GetComponent<AudioSource>();
    }

    void MakeSinglition()
    {
        if (instance != null)
        {
            Destroy(gameObject);     
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SoundOnOff()
    {
        sound = !sound;
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        if (sound)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
