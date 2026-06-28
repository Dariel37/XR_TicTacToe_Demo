using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource audioSource;
    public AudioSource musicSource;

    public AudioClip missileLaunchSound;
    public AudioClip explosionSound;
    public AudioClip hitSound;
    public AudioClip playerSelect;
    public AudioClip backgroundMusic;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
{
    PlayBackgroundMusic();
}


   public void PlayMissileLaunch()
    {
        audioSource.PlayOneShot(missileLaunchSound);
    }

    public void PlayExplosion()
    {
        audioSource.PlayOneShot(explosionSound);
    }

    public void PlayHit()
    {
        audioSource.PlayOneShot(hitSound);
    }

     public void BoardPlayerSelect()
    {
        audioSource.PlayOneShot(playerSelect);
    }

    public void PlayBackgroundMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.volume = 0.3f;
        musicSource.Play();
    }

}
