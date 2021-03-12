using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameOverOverlay : MonoBehaviour
{
    private Image _background;
    private Text _tryAgainText;
    private Text _playAgainText;
    private LayeredText _gameOverText;
    private LayeredText _victoryText;

    private void Awake()
    {
        _background = GetComponent<Image>();
        _tryAgainText = GetComponentsInChildren<Text>()[0];
        _playAgainText = GetComponentsInChildren<Text>()[1];
        _gameOverText = GetComponentsInChildren<LayeredText>()[0];
        _victoryText = GetComponentsInChildren<LayeredText>()[1];
        
        gameObject.SetActive(false);
        _tryAgainText.gameObject.SetActive(false);
        _playAgainText.gameObject.SetActive(false);
        _gameOverText.gameObject.SetActive(false);
        _victoryText.gameObject.SetActive(false);
    }

    public void Show(bool victorious)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeScreen(victorious ? Color.white : Color.black, 5));
        StartCoroutine(ShowReplayText(victorious, 4));
        if (victorious)
        {
            _victoryText.gameObject.SetActive(true);
        }
        else
        {
            _gameOverText.gameObject.SetActive(true);
        }
    }

    private IEnumerator ShowReplayText(bool victorious, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (victorious)
        {
            _playAgainText.gameObject.SetActive(true);
        }
        else
        {
            _tryAgainText.gameObject.SetActive(true);   
        }
    }

    private IEnumerator FadeScreen(Color color, float duration)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;
        float a = 0;

        while (a < 1)
        {
            a += 1.0f / (duration / 0.01f);
            _background.color = new Color(r, g, b, a);
            yield return new WaitForSeconds(0.01f);
        }
    }
}