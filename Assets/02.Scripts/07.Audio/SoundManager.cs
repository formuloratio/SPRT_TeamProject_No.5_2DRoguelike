using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    
    
    [Header("Player")]
    public AudioClip eatFood;
    public AudioClip levelUp;
    public AudioClip meleeSwing;
    public AudioClip magicShot;
    public AudioClip fireMagic;
    public AudioClip expGain;  

    [Header("Monster")]
    public AudioClip enemyLongAttack;
    public AudioClip expDrop;

    [Header("BGM")]
    public AudioClip mainBgm;    // MainScene
    public AudioClip CrtselectBgm;  // Character Select
    public AudioClip gameBgm; 

    
    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource bgmSource;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)] public float bgmVolume = 1f;  
    [Range(0f, 1f)] public float sfxVolume = 1f;  
    
    private Dictionary<AudioClip, float> sfxCooldown = new Dictionary<AudioClip, float>();

    [Header("SFX Cooldown")]
    [Tooltip("같은 사운드 연속 재생 제한 시간")]
    [SerializeField] private float defaultSfxInterval = 0.2f;

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
            return;
        }
    }
    
    private void Update()
    {
        //  Inspector에서 바꾸면 자동으로 반영
        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    public void PlaySFX(AudioClip clip, float interval = -1f)
    {
        if (clip == null) return;

        // interval을 지정 안 하면 기본값 사용
        if (interval <= 0) interval = defaultSfxInterval;

        // 이미 사운드가 등록되어 있고, 아직 재생 쿨타임이 안 지났으면 스킵
        if (sfxCooldown.ContainsKey(clip))
        {
            if (Time.time < sfxCooldown[clip])
                return;
        }

        // 플레이
        sfxSource.PlayOneShot(clip);

        // 다음 재생 시간 갱신
        sfxCooldown[clip] = Time.time + interval;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip) return; // 중복 재생 방지
            bgmSource.clip = clip;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume; 
            bgmSource.Play();
    }
}