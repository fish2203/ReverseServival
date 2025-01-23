using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;


    public AudioMixer audioMixer;

    public AudioClip bgmClip;
    AudioSource bgmPlayer;
    [HideInInspector] public float bgmVolume;

    public AudioClip[] sfxClips;
    AudioSource[] sfxPlayers;
    [HideInInspector] public float sfxVolume;

    public enum Sfx 
    { 
        Q_spawn,
        Q_hit,

        W_spawn,
        W_hit,
        W_die,

        E_spawn,
        E_shot,
        E_hit,
        E_die,

        QW_spawn,
        QW_hit,
        QW_die,

        QE_spawn,
        QE_shot,
        QE_hit,
        QE_die,

        WE_spawn,
        WE_hit,
        WE_die,

        Y_axe,
        Y_block,
        Y_hurt,
        Y_iceSphere,
        Y_levelup,
        Y_sword,

        NoMoney,
        Ruby
    };

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // bgm
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;
            bgmPlayer.volume = 1f;
            bgmPlayer.clip = bgmClip;
            bgmPlayer.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];

            // sfx
            GameObject sfxObject = new GameObject("SfxObject");
            sfxObject.transform.parent = transform;
            sfxPlayers = new AudioSource[sfxClips.Length];
            for (int i = 0; i < sfxClips.Length; i++)
            {
                sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
                sfxPlayers[i].playOnAwake = false;
                sfxPlayers[i].loop = false;
                sfxPlayers[i].volume = 1f;
                sfxPlayers[i].clip = sfxClips[i];
                sfxPlayers[i].outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            }

            bgmPlayer.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        sfxPlayers[(int)sfx].Play();
    }
}