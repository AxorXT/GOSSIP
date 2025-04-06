using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator playerAnimator; // El Animator del jugador
    public GameObject ScoreScren;
    public GameObject PantallaInicial;
    public GameObject PantallaOpciones;
    public GameObject CREDITS;
    public GameObject GameName;
    

    [Header("Velocidad")]
    public float startSpeed = 10f; // Velocidad inicial
    public float maxSpeed = 25f;   // Velocidad máxima
    public float speedIncreaseRate = 0.5f; // Cuánto sube por segundo
    private float forwardSpeed;    // Velocidad actual

    private bool gameStarted = false; // Estado del juego
    private float targetX; // Posición objetivo en X


    [Header("Carriles")]
    public float blockSize = 5f; // Tamaño del bloque
    public int maxLanes = 3; // Número de carriles
    private float minX, maxX;

    void Start()
    {
        PantallaOpciones.SetActive(false);
        PantallaInicial.SetActive(true);
        ScoreScren.SetActive(false);
        // Velocidad inicial
        forwardSpeed = startSpeed;

        // Configuración de animaciones
        playerAnimator.SetBool("IsIdle", true);
        playerAnimator.SetBool("IsWalking", false);

        // Posición inicial
        targetX = 0;
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        minX = -((maxLanes - 1) / 2f) * blockSize;
        maxX = ((maxLanes - 1) / 2f) * blockSize;
    }

    public void StartGame()
    {
    
            gameStarted = true;
            playerAnimator.SetBool("IsIdle", false);
            playerAnimator.SetBool("IsWalking", true);
            PantallaInicial.SetActive(false);
            ScoreScren.SetActive(true);

        
    }

    void Update()
    {
        // Esperar clic para comenzar
        /*if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            gameStarted = true;
            playerAnimator.SetBool("IsIdle", false);
            playerAnimator.SetBool("IsWalking", true);
            PantallaInicial.SetActive(false);
            ScoreScren.SetActive(true);

        }*/

        if (gameStarted)
        {
            // Aumentar velocidad progresivamente (hasta el máximo)
            if (forwardSpeed < maxSpeed)
            {
                forwardSpeed += speedIncreaseRate * Time.deltaTime;
            }

            // Avance
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

            // Movimiento lateral
            if (Input.GetKeyDown(KeyCode.W) && targetX - blockSize >= minX)
            {
                targetX -= blockSize;
            }
            else if (Input.GetKeyDown(KeyCode.S) && targetX + blockSize <= maxX)
            {
                targetX += blockSize;
            }

            // Suavizar movimiento lateral
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        }
    }

    public void Opciones()
    {
        PantallaInicial.SetActive(false);
        ScoreScren.SetActive(false);
        PantallaOpciones.SetActive(true);
        CREDITS.SetActive(false);

        RectTransform rt = GameName.GetComponent<RectTransform>();

        // Primero posicionarlo fuera de la pantalla (izquierda)
        rt.anchoredPosition = new Vector2(-1000f, rt.anchoredPosition.y);

        // Animarlo hacia el centro (x = 0)
        LeanTween.move(rt, new Vector2(-5f, rt.anchoredPosition.y), 1.5f).setEaseOutExpo();
    }

    public void SalirOpciones()
    {
        PantallaInicial.SetActive(true);
        ScoreScren.SetActive(false);
        PantallaOpciones.SetActive(false);
        CREDITS.SetActive(false);
    }

    public void OpenCredits()
    {
        PantallaInicial.SetActive(false);
        ScoreScren.SetActive(false);
        PantallaOpciones.SetActive(false);
        CREDITS.SetActive(true);
    }
}


