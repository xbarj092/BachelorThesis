using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsScreen : BaseScreen
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _uiSlider;

    private void Start()
    {
        Debug.Log(AudioManager.Instance.GetSoundVolume(SoundGroup.None));
        Debug.Log(AudioManager.Instance.GetSoundVolume(SoundGroup.SFX));
        Debug.Log(AudioManager.Instance.GetSoundVolume(SoundGroup.Music));
        Debug.Log(AudioManager.Instance.GetSoundVolume(SoundGroup.UI));
        _masterSlider.value = AudioManager.Instance.GetSoundVolume(SoundGroup.None);
        _sfxSlider.value = AudioManager.Instance.GetSoundVolume(SoundGroup.SFX);
        _musicSlider.value = AudioManager.Instance.GetSoundVolume(SoundGroup.Music);
        _uiSlider.value = AudioManager.Instance.GetSoundVolume(SoundGroup.UI);
    }

    public void SetMasterVolume(float volume)
    {
        AudioManager.Instance.SetSoundVolume(SoundGroup.None, volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSoundVolume(SoundGroup.SFX, volume);
    }

    public void SetMusicVolume(float volume)
    {
        AudioManager.Instance.SetSoundVolume(SoundGroup.Music, volume);
    }

    public void SetUIVolume(float volume)
    {
        AudioManager.Instance.SetSoundVolume(SoundGroup.UI, volume);
    }

    public void ResetVolume()
    {
        SetMasterVolume(0);
        SetSFXVolume(0);
        SetMusicVolume(0);
        SetUIVolume(0);
    }
}
