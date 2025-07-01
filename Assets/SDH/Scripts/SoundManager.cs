using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public enum SFX
    {

    }
    public enum BGM
    { 

    }


    public AudioClip[] bgmClips;
    float bgmVolume = 0.5f;
    AudioSource bgmPlayer;


    public AudioClip[] sfxClips;
    float sfxVolume = 0.5f;
    int initChannels = 10; // 게임 스케일 따라 초기 채널 수 조정
    List<AudioSource> sfxPool = new List<AudioSource>();

    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        InitBGM();
        InitSFX();
    }


    #region BGM관리
    void InitBGM()
    {
        GameObject bgmObject = new GameObject("BGMPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
    }


    public void PlayBgm(BGM bgm)
    {
        int bgmIndex = (int)bgm;
        if (bgmIndex < 0 || bgmIndex >= bgmClips.Length)
        {
            return;
        }
        bgmPlayer.clip = bgmClips[bgmIndex];
        bgmPlayer.Play();
    }
    public void StopBgm()
    {
        bgmPlayer.Stop();
    }
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmPlayer.volume = bgmVolume;
    }
    #endregion

    #region SFX관리
    void InitSFX()
    {
        for (int i = 0; i < initChannels; i++)
        {
            GameObject sfxObj = new GameObject("SFXPlayer" + i);
            sfxObj.transform.parent = transform;
            AudioSource source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.volume = sfxVolume;
            sfxPool.Add(source);
        }
    }
    public void PlaySfx(SFX sfx, bool isLoop = false)
    {
        int sfxIndex = (int)sfx;

        if (sfxIndex < 0 || sfxIndex >= sfxClips.Length)
        {
            return;
        }

        AudioSource source = sfxPool.Find(s => !s.isPlaying);

        if (source == null)
        {
            GameObject sfxObj = new GameObject("SFXPlayer_New");
            sfxObj.transform.parent = transform;
            source = sfxObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.volume = sfxVolume;
            sfxPool.Add(source);
        }

        source.clip = sfxClips[sfxIndex];
        source.loop = isLoop;
        source.Play();
    }

    public void StopSfx(SFX sfx)
    {
        int sfxIndex = (int)sfx;
        foreach (AudioSource source in sfxPool)
        {
            if (source.clip == sfxClips[sfxIndex])
            {
                source.Stop();
            }
        }
    }
    public void StopAllSfx()
    {
        foreach (AudioSource source in sfxPool)
        {
            source.Stop();
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        foreach (AudioSource source in sfxPool)
        {
            source.volume = sfxVolume;
        }
    }
    #endregion
}
