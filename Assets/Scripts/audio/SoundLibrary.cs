using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Blackjack/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    public AudioClip card;
    public AudioClip chip;
    public AudioClip click;
    public AudioClip win;
    public AudioClip lose;
    public AudioClip ambient;

    [Range(0f, 1f)] public float volume = 0.8f;
    [Range(0f, 1f)] public float ambientVolume = 0.25f;

    [Header("settings")]
    public string soundPrefKey = "sound_on";
    public string musicPrefKey = "music_on";
    public string soundLabelFormat = "SOUND: {0}";
    public string musicLabelFormat = "MUSIC: {0}";
    public string onText = "ON";
    public string offText = "OFF";

    public bool SoundOn
    {
        get => PlayerPrefs.GetInt(soundPrefKey, 1) == 1;
        set => PlayerPrefs.SetInt(soundPrefKey, value ? 1 : 0);
    }

    public bool MusicOn
    {
        get => PlayerPrefs.GetInt(musicPrefKey, 1) == 1;
        set => PlayerPrefs.SetInt(musicPrefKey, value ? 1 : 0);
    }
}
