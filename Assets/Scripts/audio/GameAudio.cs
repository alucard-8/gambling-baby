using UnityEngine;
using UnityEngine.UI;

public class GameAudio : MonoBehaviour
{
    [SerializeField] private BlackjackGame game;
    [SerializeField] private SoundLibrary sounds;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioSource ambientSource;
    [SerializeField] private Button[] clickButtons;

    private int _lastBet;

    private void Start()
    {
        game.OnCardDealt += OnCardDealt;
        game.OnDealerHoleRevealed += PlayCard;
        game.OnRoundEnded += OnRoundEnded;
        game.Bettor.Bet.OnChanged += OnBetChanged;
        _lastBet = game.Bettor.Bet.Value;

        AudioListener.volume = sounds.SoundOn ? 1f : 0f;

        if (sounds.ambient != null && sounds.MusicOn)
        {
            ambientSource.clip = sounds.ambient;
            ambientSource.volume = sounds.ambientVolume;
            ambientSource.loop = true;
            ambientSource.Play();
        }

        foreach (Button button in clickButtons)
            button.onClick.AddListener(PlayClick);
    }

    private void OnDestroy()
    {
        game.OnCardDealt -= OnCardDealt;
        game.OnDealerHoleRevealed -= PlayCard;
        game.OnRoundEnded -= OnRoundEnded;
        if (game.Bettor != null)
            game.Bettor.Bet.OnChanged -= OnBetChanged;

        foreach (Button button in clickButtons)
            button.onClick.RemoveListener(PlayClick);
    }

    private void OnCardDealt(CardData card, bool isPlayer, bool faceDown) => PlayCard();

    private void OnBetChanged(int bet)
    {
        if (bet > _lastBet)
            Play(sounds.chip);
        _lastBet = bet;
    }

    private void OnRoundEnded(RoundResult result)
    {
        if (result == RoundResult.Win)
            Play(sounds.win);
        else if (result == RoundResult.Lose)
            Play(sounds.lose);
    }

    private void PlayCard() => Play(sounds.card);
    private void PlayClick() => Play(sounds.click);

    private void Play(AudioClip clip)
    {
        if (clip != null)
            source.PlayOneShot(clip, sounds.volume);
    }
}
