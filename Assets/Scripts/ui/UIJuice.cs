using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIJuice : MonoBehaviour
{
    [SerializeField] private BlackjackGame game;
    [SerializeField] private UITheme theme;
    [SerializeField] private Transform playerCards;
    [SerializeField] private Transform dealerCards;
    [SerializeField] private Transform messageText;
    [SerializeField] private TMP_Text tokensText;
    [SerializeField] private Button[] hoverButtons;

    private readonly Dictionary<Transform, float> _hoverTargets = new Dictionary<Transform, float>();
    private Coroutine _countRoutine;
    private int _shownTokens;

    private void Start()
    {
        game.OnCardDealt += OnCardDealt;
        game.Message.OnChanged += OnMessageChanged;
        game.Bettor.Tokens.OnChanged += OnTokensChanged;

        _shownTokens = game.Bettor.Tokens.Value;
        tokensText.text = string.Format(theme.tokensFormat, _shownTokens);

        foreach (Button button in hoverButtons)
            AddHover(button.transform);
    }

    private void OnDestroy()
    {
        game.OnCardDealt -= OnCardDealt;
        game.Message.OnChanged -= OnMessageChanged;
        if (game.Bettor != null) game.Bettor.Tokens.OnChanged -= OnTokensChanged;
    }

    private void Update()
    {
        foreach (KeyValuePair<Transform, float> pair in _hoverTargets)
            pair.Key.localScale = Vector3.one * Mathf.Lerp(pair.Key.localScale.x, pair.Value, Time.unscaledDeltaTime * theme.buttonHoverSpeed);
    }

    private void OnCardDealt(CardData card, bool isPlayer, bool faceDown) => StartCoroutine(PopNewCard(isPlayer ? playerCards : dealerCards));
    private void OnMessageChanged(string message) => StartCoroutine(Scale(messageText, theme.messagePunchScale, 1f, theme.messagePunchDuration));

    private void OnTokensChanged(int tokens)
    {
        if (_countRoutine != null) StopCoroutine(_countRoutine);
        _countRoutine = StartCoroutine(CountTo(tokens));
    }

    private IEnumerator PopNewCard(Transform row)
    {
        yield return null;
        if (row.childCount > 0)
            yield return Scale(row.GetChild(row.childCount - 1), theme.cardPopStartScale, 1f, theme.cardPopDuration);
    }

    private IEnumerator Scale(Transform target, float from, float to, float duration)
    {
        for (float t = 0f; t < 1f && target != null; t += Time.deltaTime / duration)
        {
            target.localScale = Vector3.one * Mathf.LerpUnclamped(from, to, theme.popCurve.Evaluate(t));
            yield return null;
        }
        if (target != null) target.localScale = Vector3.one * to;
    }

    private IEnumerator CountTo(int target)
    {
        int start = _shownTokens;
        for (float t = 0f; t < 1f; t += Time.deltaTime / theme.tokenCountDuration)
        {
            _shownTokens = Mathf.RoundToInt(Mathf.Lerp(start, target, t));
            tokensText.text = string.Format(theme.tokensFormat, _shownTokens);
            yield return null;
        }
        _shownTokens = target;
        tokensText.text = string.Format(theme.tokensFormat, target);
    }

    private void AddHover(Transform target)
    {
        _hoverTargets[target] = 1f;
        EventTrigger trigger = target.gameObject.AddComponent<EventTrigger>();
        foreach (EventTriggerType type in new[] { EventTriggerType.PointerEnter, EventTriggerType.PointerExit })
        {
            var entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => _hoverTargets[target] = type == EventTriggerType.PointerEnter ? theme.buttonHoverScale : 1f);
            trigger.triggers.Add(entry);
        }
    }
}
