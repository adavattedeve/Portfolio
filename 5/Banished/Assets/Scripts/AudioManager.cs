using UnityEngine;
using System.Collections;
[System.Serializable]
public class AudioClipAndAmplitude {
    public AudioClip clip;
    public float startTime = 0f;
    public float amplitude = 1f;
}
public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    public AudioSource shootSource;
    public AudioClipAndAmplitude pistolShoot;
    public AudioClipAndAmplitude shotgunShoot;

    public AudioSource uiSource;
    public AudioClipAndAmplitude onButtonClick;
    public AudioClipAndAmplitude mouseOnButton;

    public AudioSource collideSource;
    public AudioClipAndAmplitude bulletHitChar;
    public AudioClipAndAmplitude bulletHitWall;


    public AudioSource musicSource;
    public AudioClipAndAmplitude song;

    public AudioSource othersSource;
    public AudioClipAndAmplitude playerDeath;
    public AudioClipAndAmplitude ritualCompleted;


    private AudioClip currentSong;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GameplaySong();
        }
    }
    #region Songs
    public void MainMenuSong()
    {
        Debug.Log("MainMenu song playeing");
    }
    public void GameplaySong()
    {
        musicSource.clip = song.clip;
        musicSource.loop = true;
        musicSource.Play();
        
    }
    #endregion
    #region Gameplay Audio
    public void CharacterHitWithBullet()
    {
        collideSource.clip = bulletHitChar.clip;
        collideSource.time = bulletHitChar.startTime;
        collideSource.Play();
        //collideSource.PlayOneShot(, bulletHitChar.amplitude);
    }
    public void WallHitWithBullet()
    {
        collideSource.clip = bulletHitWall.clip;
        collideSource.time = bulletHitWall.startTime;
        collideSource.Play();
        //collideSource.PlayOneShot(bulletHitWall.clip, bulletHitWall.amplitude);
    }
    public void PlayerDeath()
    {
        othersSource.PlayOneShot(playerDeath.clip, playerDeath.amplitude);
    }

    public void RitualCompleted()
    {
        othersSource.PlayOneShot(ritualCompleted.clip, ritualCompleted.amplitude);
    }
    public void PistolShoot()
    {
        shootSource.PlayOneShot(pistolShoot.clip, pistolShoot.amplitude);
    }
    public void ShotgunShoot()
    {
        shootSource.PlayOneShot(shotgunShoot.clip, shotgunShoot.amplitude);
    }
    #endregion

    #region UI Audio
    public void MouseOnButton()
    {
        uiSource.PlayOneShot(mouseOnButton.clip, mouseOnButton.amplitude);
    }
    public void ButtonClicked()
    {
        uiSource.PlayOneShot(onButtonClick.clip, onButtonClick.amplitude);
    }
    #endregion
}
