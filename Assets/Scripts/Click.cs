using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class Click : MonoBehaviour
{
    public TMP_Text scoreText; // Referencia al Text donde se mostrará el puntaje
    public TMP_Text feedbackText; // Referencia al Text para mostrar el mensaje de guardado
    private int score;
    private int scoreThreshold = 300; // Cada 300 puntos registrar automáticamente el puntaje

    public Button clickableButton; // Referencia al botón que se puede hacer clic
    public Image clickableImage; // Referencia al componente Image
    public List<Sprite> sprites; // Lista de sprites para cambiar
    public AudioClip clickSound; // Clip de audio para el sonido de clic
    public AudioSource audioSource; // Componente AudioSource para reproducir el sonido

    public DBScoreSender dbScoreSender; // Referencia al script que envía datos a la base de datos
    public UserSessionData userSessionData; // Datos del usuario para registrar el puntaje

    public float messageTime = 3; // Tiempo que se mostrará el mensaje de guardado

    private Vector3 originalScale;

    private void Start()
    {
        score = 0;
        UpdateScoreText();
        originalScale = clickableButton.transform.localScale;
        audioSource.clip = clickSound; // Asignar el clip de sonido
        feedbackText.text = ""; // Iniciar con texto vacío

        // Añadir un evento al objeto clickeable
        clickableButton.onClick.AddListener(OnObjectClicked);
    }

    public void ReceiveUserId(string userID)
    {
        userSessionData.userId = int.Parse(userID);
        Debug.Log("user id: " + userID);
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

        // Registrar automáticamente el puntaje si alcanza el múltiplo de 300
        if (score % scoreThreshold == 0)
        {
            RegisterScore();
        }
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

    private IEnumerator ScaleObject(GameObject obj)
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

    public void RegisterScore()
    {
        if (userSessionData.userId == 0)
        {
            Debug.LogWarning("User ID no configurado. No se puede registrar el puntaje.");
            return;
        }

        // Llamar al script para enviar el puntaje a la base de datos
        dbScoreSender.SendScoreToDatabase(score, userSessionData.userId);

        // Mostrar el mensaje de retroalimentación
        StartCoroutine(ShowFeedbackMessage($"¡Puntaje guardado! Score: {score}"));
    }

    private IEnumerator ShowFeedbackMessage(string message)
    {
        feedbackText.text = message; // Mostrar el mensaje
        yield return new WaitForSeconds(messageTime); // Esperar 3 segundos
        feedbackText.text = ""; // Volver al texto vacío
    }
}