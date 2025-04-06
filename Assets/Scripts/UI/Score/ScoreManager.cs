using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public Transform player; // Jugador (para usar su posición Z como score)
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    private float currentScore = 0f;
    private float highScore = 0f;

    void Start()
    {
        highScore = PlayerPrefs.GetFloat("HIGHSCORE", 0f);
    }

    void Update()
    {
        // Suponiendo que el score es la distancia recorrida en Z
        currentScore = player.position.z;

        // Actualizamos textos en pantalla
        scoreText.text = "SCORE: " + Mathf.FloorToInt(currentScore).ToString();
        highScoreText.text = "HIGHSCORE: " + Mathf.FloorToInt(highScore).ToString();

        // Verificamos si hay nuevo récord
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetFloat("HIGHSCORE", highScore);
        }
    }

    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("HIGHSCORE");
        highScore = 0;
    }
}

