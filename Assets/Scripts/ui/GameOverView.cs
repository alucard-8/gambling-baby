using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [SerializeField] private BlackjackGame game;
    [SerializeField] private GameTexts texts;
    [SerializeField] private string menuSceneName = "MainMenu";

    [SerializeField] private GameObject panel;
    [SerializeField] private Button panelButton;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text hintText;

    private void Start()
    {
        titleText.text = texts.gameOverTitle;
        hintText.text = texts.gameOverHint;

        game.State.OnChanged += OnStateChanged;
        panelButton.onClick.AddListener(BackToMenu);

        OnStateChanged(game.State.Value);
    }

    private void OnDestroy()
    {
        game.State.OnChanged -= OnStateChanged;
        panelButton.onClick.RemoveListener(BackToMenu);
    }

    private void OnStateChanged(GameState state)
    {
        panel.SetActive(state == GameState.GameOver);
    }

    private void BackToMenu()
    {
        SceneManager.LoadScene(menuSceneName);
    }
}
