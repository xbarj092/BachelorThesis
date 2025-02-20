public class AudioSettingsScreen : BaseScreen
{
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
}
