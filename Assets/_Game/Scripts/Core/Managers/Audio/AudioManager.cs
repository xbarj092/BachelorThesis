using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoSingleton<AudioManager>
{
    [SerializeField] private bool _spacialBlend;
    [SerializeField] private SerializedDictionary<SoundGroup, AudioMixerGroup> _mixers = new();
    [SerializeField] private AudioMixerGroup _mixer;
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
    }

    public void Play(SoundType name)
    {
        Sound sound = _sounds.FirstOrDefault(sound => sound.Name == name);
        bool find = false;

        AudioSource foundSource = sound.Source.Find((source) => {
            find = !source.isPlaying;
            return find;
        });

        if (foundSource != null)
        {
            foundSource.Play();
        }
        else
        {
            sound.Source[0].Play();
        }
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
        _mixer.audioMixer.SetFloat(GetNameFromGroup(group), volume);
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
