using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private bool _spacialBlend;
    [SerializeField] private SerializedDictionary<SoundGroup, AudioMixerGroup> _mixers = new();
    [SerializeField] private AudioMixerGroup _masterMixer;
    [SerializeField] private List<Sound> _sounds;

    public bool Muted = false;

    private const string MASTER_VOLUME = "MasterVolume";
    private const string SFX_VOLUME = "SFXVolume";
    private const string MUSIC_VOLUME = "MusicVolume";
    private const string UI_VOLUME = "UIVolume";

    private void Awake()
    {
        foreach (Sound sound in _sounds)
        {
            for (int i = 0; i < sound.NumberOfSource; i++)
            { 
                AudioSource source = gameObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.loop = sound.Group == SoundGroup.Music;
                source.clip = sound.Clip;
                source.volume = sound.Volume;
                source.pitch = sound.Pitch;
                source.spatialBlend = sound.SpatialBlend;
                source.outputAudioMixerGroup = _mixers[sound.Group];
                sound.Source.Add(source);
            }
        }

        StartCoroutine(SetAudioMixerValues());
    }

    private IEnumerator SetAudioMixerValues()
    {
        yield return new WaitForSeconds(0.1f);

        Dictionary<SoundGroup, float> soundSettings = LocalDataStorage.Instance.PlayerPrefs.LoadAudioSettings();
        if (soundSettings != null)
        {
            foreach (KeyValuePair<SoundGroup, float> soundSetting in soundSettings)
            {
                _masterMixer.audioMixer.SetFloat(GetNameFromGroup(soundSetting.Key), soundSetting.Value);
            }
        }
        else
        {
            soundSettings = new()
            {
                { SoundGroup.None, 0},
                { SoundGroup.Music, 0},
                { SoundGroup.SFX, 0},
                { SoundGroup.UI, 0},
            };
            LocalDataStorage.Instance.PlayerPrefs.SaveAudioSettings(soundSettings);
        }
    }

    public void Play(SoundType name)
    {
        Sound sound = _sounds.FirstOrDefault(sound => sound.Name == name);
        if (sound == null) return;

        bool find = false;

        AudioSource foundSource = sound.Source.Find((source) => {
            find = !source.isPlaying;
            return find;
        });

        if (foundSource == null)
        {
            foundSource = sound.Source[0];
        }

        if (foundSource != null)
        {
            StartCoroutine(PlayWithPitchVariation(foundSource, sound.VariablePitch));
        }
    }

    private IEnumerator PlayWithPitchVariation(AudioSource source, bool variablePitch)
    {
        float originalPitch = source.pitch;

        if (variablePitch)
        {
            source.pitch += UnityEngine.Random.Range(-0.08f, 0.09f);
        }

        source.Play();
        yield return new WaitForSeconds(source.clip.length);
        source.pitch = originalPitch;
    }

    public void Stop(SoundType name)
    {
        Sound foundSound = _sounds.FirstOrDefault(sound => sound.Name == name);
        foundSound.Source.ForEach((source) => source.Stop());
    }

    public bool IsPlaying(SoundType name) 
    { 
        Sound foundSound = _sounds.FirstOrDefault(sound => sound.Name == name);
        return foundSound.Source.Exists((source) => source.isPlaying);
    }

    public void SetSoundVolume(SoundGroup group, float volume)
    {
        _masterMixer.audioMixer.SetFloat(GetNameFromGroup(group), volume);
        Dictionary<SoundGroup, float> soundSettings = LocalDataStorage.Instance.PlayerPrefs.LoadAudioSettings();
        soundSettings[group] = volume;
        LocalDataStorage.Instance.PlayerPrefs.SaveAudioSettings(soundSettings);
    }

    public float GetSoundVolume(SoundGroup group)
    {
        _masterMixer.audioMixer.GetFloat(GetNameFromGroup(group), out float volume);
        return volume;
    }

    private string GetNameFromGroup(SoundGroup group)
    {
        return group switch
        {
            SoundGroup.None => MASTER_VOLUME,
            SoundGroup.SFX => SFX_VOLUME,
            SoundGroup.Music => MUSIC_VOLUME,
            SoundGroup.UI => UI_VOLUME,
            _ => null
        };
    }
}
