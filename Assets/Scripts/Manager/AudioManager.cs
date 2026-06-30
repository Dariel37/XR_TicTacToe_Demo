using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton reference so any script can access audio easily.
    public static AudioManager Instance { get; private set; }
    // Audio source for sound effects.
    public AudioSource audioSource;
     // Separate audio source for background music.
    public AudioSource musicSource;

    // Sound effect clips.
    public AudioClip missileLaunchSound;
    public AudioClip explosionSound;
    public AudioClip hitSound;
    public AudioClip playerSelect;

    // Background music clip.
    public AudioClip backgroundMusic;

    private void Awake()
    {
        // Register singleton instance.
        Instance = this;
    }

    private void Start()
    {
        // Start background music automatically when game begins.
        PlayBackgroundMusic();
    }


   public void PlayMissileLaunch()
    {
        // Play missile launch sound.
        audioSource.PlayOneShot(missileLaunchSound);
    }

    public void PlayExplosion()
    {
        // Play explosion sound.
        audioSource.PlayOneShot(explosionSound);
    }

    public void PlayHit()
    {
        // Play hit sound.
        audioSource.PlayOneShot(hitSound);
    }

     public void BoardPlayerSelect()
    {
        // Play board selection sound.
        audioSource.PlayOneShot(playerSelect);
    }

    public void PlayBackgroundMusic()
    {
        // Assign background music clip.
        musicSource.clip = backgroundMusic;
        // Loop forever.
        musicSource.loop = true;
        // Lower music volume.
        musicSource.volume = 0.3f;
        // Start playing music.
        musicSource.Play();
    }

}
