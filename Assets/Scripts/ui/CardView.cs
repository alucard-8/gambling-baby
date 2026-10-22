using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private RectTransform visual;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text rankText;

    private CardData _card;
    private UITheme _theme;

    public void Setup(CardData card, bool faceDown, UITheme theme, Transform deckPoint)
    {
        _card = card;
        _theme = theme;
        ShowFace(!faceDown);
        StartCoroutine(SlideIn(deckPoint));
    }

    public void Reveal() => StartCoroutine(Flip());

    private void ShowFace(bool faceUp)
    {
        image.sprite = faceUp ? _card.sprite : _theme.cardBack;
        rankText.text = faceUp && _card.sprite == null ? _card.rankName : "";
    }

    private IEnumerator SlideIn(Transform from)
    {
        image.enabled = false;
        yield return null;

        visual.position = from.position;
        Vector3 start = visual.localPosition;
        image.enabled = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / _theme.cardSlideDuration;
            visual.localPosition = Vector3.Lerp(start, Vector3.zero, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        visual.localPosition = Vector3.zero;
    }

    private IEnumerator Flip()
    {
        Vector3 scale = visual.localScale;
        float halfDuration = _theme.cardFlipDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            scale.x = Mathf.Lerp(1f, 0f, Mathf.Clamp01(elapsed / halfDuration));
            visual.localScale = scale;
            yield return null;
        }

        scale.x = 0f;
        visual.localScale = scale;
        ShowFace(true);

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            scale.x = Mathf.Lerp(0f, 1f, Mathf.Clamp01(elapsed / halfDuration));
            visual.localScale = scale;
            yield return null;
        }

        scale.x = 1f;
        visual.localScale = scale;
    }
}
