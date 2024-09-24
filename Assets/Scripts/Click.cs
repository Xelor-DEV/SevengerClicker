using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
public class Click : MonoBehaviour
{
    public TMP_Text scoreText; // Referencia al Text donde se mostrará el puntaje
    private int score;

    public Button clickableButton; // Referencia al botón que se puede hacer clic
    public Image clickableImage; // Referencia al componente Image
    public List<Sprite> sprites; // Lista de sprites para cambiar
    public AudioClip clickSound; // Clip de audio para el sonido de clic
    public AudioSource audioSource; // Componente AudioSource para reproducir el sonido

    private Vector3 originalScale;

    private void Start()
    {
        score = 0;
        UpdateScoreText();
        originalScale = clickableButton.transform.localScale;
        audioSource.clip = clickSound; // Asignar el clip de sonido

        // Añadir un evento al objeto clickeable
        clickableButton.onClick.AddListener(OnObjectClicked);
    }

    private void OnObjectClicked()
    {
        IncreaseScore(10); // Incrementar el puntaje en 10
        audioSource.Play(); // Reproducir el sonido de clic
        StartCoroutine(ScaleObject(clickableButton.gameObject)); // Escalar el objeto al hacer clic
    }

    private void IncreaseScore(int amount)
    {
        score += amount;
        UpdateScoreText();
        UpdateSprite();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Puntaje: " + score.ToString();
    }

    private void UpdateSprite()
    {
        if (score >= 500)
        {
            clickableImage.sprite = sprites[2]; // Sprite para 500 puntos
        }
        else if (score >= 100)
        {
            clickableImage.sprite = sprites[1]; // Sprite para 100 puntos
        }
        else
        {
            clickableImage.sprite = sprites[0]; // Sprite para 0-99 puntos
        }
    }

    private System.Collections.IEnumerator ScaleObject(GameObject obj)
    {
        Vector3 targetScale = originalScale * 1.2f; // Aumentar el tamaño
        float duration = 0.2f; // Duración de la animación
        float elapsed = 0f;

        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Volver al tamaño original
        elapsed = 0f;
        while (elapsed < duration)
        {
            obj.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que el objeto regrese exactamente a su escala original
        obj.transform.localScale = originalScale;
    }
    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
