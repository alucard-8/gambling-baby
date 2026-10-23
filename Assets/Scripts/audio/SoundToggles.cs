using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundToggles : MonoBehaviour
{
    [SerializeField] private SoundLibrary sounds;
    [SerializeField] private Button soundButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private TMP_Text soundText;
    [SerializeField] private TMP_Text musicText;
    [SerializeField] private AudioSource musicSource;

    private void Start()
    {
        musicSource.clip = sounds.ambient;
        musicSource.volume = sounds.ambientVolume;
        musicSource.loop = true;

        soundButton.onClick.AddListener(ToggleSound);
        musicButton.onClick.AddListener(ToggleMusic);
        Refresh();
    }

    private void OnDestroy()
    {
        soundButton.onClick.RemoveListener(ToggleSound);
        musicButton.onClick.RemoveListener(ToggleMusic);
    }

    private void ToggleSound()
    {
        sounds.SoundOn = !sounds.SoundOn;
        Refresh();
    }

    private void ToggleMusic()
    {
        sounds.MusicOn = !sounds.MusicOn;
        Refresh();
    }

    private void Refresh()
    {
        AudioListener.volume = sounds.SoundOn ? 1f : 0f;

        bool playMusic = sounds.MusicOn && musicSource.clip != null;
        if (playMusic && !musicSource.isPlaying) musicSource.Play();
        else if (!playMusic && musicSource.isPlaying) musicSource.Stop();

        soundText.text = string.Format(sounds.soundLabelFormat, sounds.SoundOn ? sounds.onText : sounds.offText);
        musicText.text = string.Format(sounds.musicLabelFormat, sounds.MusicOn ? sounds.onText : sounds.offText);
    }
}
