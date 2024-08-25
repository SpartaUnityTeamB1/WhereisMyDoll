using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    private AudioMixer audioMixer;
    private List<AudioMixerGroup> audioMixerGroups;

    private AudioSource[] bgmSources = new AudioSource[2];
    private Queue<AudioSource> sfxSources = new Queue<AudioSource>();
    private AudioSource[] plusSFXSources = new AudioSource[2];
    private AudioSource dialogueSource;

    [Range(0.0001f, 1f)] private List<float> groupVolumes = new List<float>();

    private GameObject obj = null;
    private AudioSource source;

    private Coroutine changeCor = null;

    private float current;
    private float lerpTime = 1f;

    private int queueCount;

    public void InitializeSound(AudioMixer audioMixer, List<AudioMixerGroup> audioMixerGroups, int queueCount)
    {
        this.audioMixer = audioMixer;
        this.audioMixerGroups = audioMixerGroups;

        this.queueCount = queueCount;

        MakeObject();
        SettingSources();

        for (int i = 0; i < audioMixerGroups.Count; ++i)
            groupVolumes.Add(PlayerPrefs.GetFloat(((MixerGroup)i).ToString(), 1f));

        groupVolumes[(int)MixerGroup.PlusSFX] = 0.0001f;
    }

    private void MakeObject()
    {
        if (null == obj)
        {
            obj = new GameObject(typeof(SoundManager).Name);

            GameManager.Instance.DontDestroy(obj);
        }
    }

    private void SettingSources()
    {
        for (int i = 0; i < bgmSources.Length; ++i)
        {
            bgmSources[i] = obj.AddComponent<AudioSource>();

            bgmSources[i].loop = true;
            bgmSources[i].outputAudioMixerGroup = audioMixerGroups[(int)MixerGroup.BGM];
        }

        for (int i = 0; i < plusSFXSources.Length; ++i)
        {
            plusSFXSources[i] = obj.AddComponent<AudioSource>();

            plusSFXSources[i].loop = true;
            plusSFXSources[i].outputAudioMixerGroup = audioMixerGroups[(int)MixerGroup.PlusSFX];
        }

        for (int i = 0; i < queueCount; ++i)
        {
            source = obj.AddComponent<AudioSource>();
            source.loop = false;
            source.outputAudioMixerGroup = audioMixerGroups[(int)MixerGroup.SFX];

            sfxSources.Enqueue(source);
        }

        dialogueSource = obj.AddComponent<AudioSource>();
        dialogueSource.loop = false;
        dialogueSource.outputAudioMixerGroup = audioMixerGroups[(int)MixerGroup.SFX];
    }

    public void PlayBGM(AudioClip bgmClip)
    {
        if (null != changeCor)
        {
            GameManager.Instance.CoroutineStop(changeCor);
            changeCor = null;
        }

        if (!bgmSources[0].isPlaying)
        {
            bgmSources[0].clip = bgmClip;
            changeCor = GameManager.Instance.CoroutineStart(ChangeBGMClip(bgmSources[0], bgmSources[1]));
        }
        else
        {
            bgmSources[1].clip = bgmClip;
            changeCor = GameManager.Instance.CoroutineStart(ChangeBGMClip(bgmSources[1], bgmSources[0]));
        }
    }

    IEnumerator ChangeBGMClip(AudioSource target, AudioSource turnOff)
    {
        current = 0f;

        target.Play();

        while (current < lerpTime)
        {
            current += Time.deltaTime;

            target.volume = Mathf.Lerp(0, 1, (current / lerpTime));
            turnOff.volume = Mathf.Lerp(1, 0, (current / lerpTime));

            yield return null;
        }

        target.volume = 1f;
        turnOff.Stop();
        turnOff.clip = null;

        changeCor = null;
    }

    public void StopBGM()
    {
        foreach (var source in bgmSources)
            source.Stop();
    }

    public void StopPlusSFX()
    {
        foreach (var source in plusSFXSources)
            source.Stop();
    }

    public void PlayPlusSFX(AudioClip plusSFXClip)
    {
        plusSFXSources[0].clip = plusSFXClip;
        plusSFXSources[0].Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        foreach (var sfxSource in sfxSources)
        {
            if (!sfxSource.isPlaying)
            {
                sfxSource.PlayOneShot(clip);
                return;
            }
        }
      
        source = sfxSources.Dequeue();
        source.Stop();

        source.PlayOneShot(clip);

        sfxSources.Enqueue(source);
    }

    public void PlayDialogueSFX(AudioClip clip)
    {
        if (dialogueSource.isPlaying)
            dialogueSource.Stop();

        dialogueSource.PlayOneShot(clip);
    }

    public void SetBGMVolume(float volume)
    {
        groupVolumes[(int)MixerGroup.BGM] = volume;
        SettingVolumes();
        PlayerPrefs.SetFloat((MixerGroup.BGM).ToString(), groupVolumes[(int)MixerGroup.BGM]);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        groupVolumes[(int)MixerGroup.SFX] = volume;
        SettingVolumes();
        PlayerPrefs.SetFloat((MixerGroup.SFX).ToString(), groupVolumes[(int)MixerGroup.SFX]);
        PlayerPrefs.Save();
    }

    public void SetPlusSFXVolume(float increaseVolume)
    {
        groupVolumes[(int)MixerGroup.PlusSFX] += increaseVolume;
        SettingVolumes();
    }

    public float GetBGMVolume()
    {
        return groupVolumes[(int)MixerGroup.BGM];
    }

    public float GetSFXVolume()
    {
        return groupVolumes[(int)MixerGroup.SFX];
    }

    public void SettingVolumes()
    {
        for (int i = 0; i < groupVolumes.Count; ++i)
            audioMixer.SetFloat(((MixerGroup)i).ToString(), Mathf.Log10(groupVolumes[i]) * 20f);
    }
}