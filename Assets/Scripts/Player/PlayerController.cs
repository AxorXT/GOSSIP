using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject ScoreScren;
    public GameObject PantallaInicial;
    public GameObject PantallaOpciones;
    public GameObject CREDITS;
    public GameObject GameName;

    [Header("Velocidad")]
    public float startSpeed = 10f;
    public float maxSpeed = 25f;
    public float speedIncreaseRate = 0.5f;
    private float forwardSpeed;

    private bool gameStarted = false;
    private float targetX;

    [Header("Carriles")]
    public float blockSize = 5f;
    public int maxLanes = 3;
    private float minX, maxX;

    public static bool GameStartedGlobally = false;

    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool swipeDetected = false;

    void Start()
    {
        PantallaOpciones.SetActive(false);
        PantallaInicial.SetActive(true);
        ScoreScren.SetActive(false);

        forwardSpeed = startSpeed;
        targetX = 0;
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        minX = -((maxLanes - 1) / 2f) * blockSize;
        maxX = ((maxLanes - 1) / 2f) * blockSize;

        GameStartedGlobally = false;
    }

    void Update()
    {
        // Tocar o dar clic para iniciar el juego
        if (!gameStarted && (
            (Application.isMobilePlatform && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
            (!Application.isMobilePlatform && Input.GetMouseButtonDown(0))
        ))
        {
            StartGame();
        }

        if (gameStarted)
        {
            // Detectar swipe en móvil
            if (Application.isMobilePlatform && Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPos = touch.position;
                    swipeDetected = false;
                }
                else if (touch.phase == TouchPhase.Moved && !swipeDetected)
                {
                    endTouchPos = touch.position;
                    DetectSwipe();
                }
            }

            // Simular swipe con mouse en el editor
            if (!Application.isMobilePlatform)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    startTouchPos = Input.mousePosition;
                    swipeDetected = false;
                }
                else if (Input.GetMouseButton(0) && !swipeDetected)
                {
                    endTouchPos = Input.mousePosition;
                    DetectSwipe();
                }
            }

            // Aumentar velocidad progresiva
            if (forwardSpeed < maxSpeed)
            {
                forwardSpeed += speedIncreaseRate * Time.deltaTime;
            }

            // Movimiento hacia adelante
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);

            // Movimiento lateral suavizado
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
        }
    }

    void DetectSwipe()
    {
        float deltaX = endTouchPos.x - startTouchPos.x;
        float deltaY = Mathf.Abs(endTouchPos.y - startTouchPos.y); // evitar diagonales

        if (Mathf.Abs(deltaX) > 100f && deltaY < 100f) // swipe horizontal claro
        {
            if (deltaX > 0 && targetX + blockSize <= maxX)
            {
                targetX += blockSize;
            }
            else if (deltaX < 0 && targetX - blockSize >= minX)
            {
                targetX -= blockSize;
            }

            swipeDetected = true;
        }
    }

    void StartGame()
    {
        gameStarted = true;
        GameStartedGlobally = true;

        PantallaInicial.SetActive(false);
        ScoreScren.SetActive(true);
    }

    public void Opciones()
    {
        PantallaInicial.SetActive(false);
        ScoreScren.SetActive(false);
        PantallaOpciones.SetActive(true);
        CREDITS.SetActive(false);

        RectTransform rt = GameName.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-1000f, rt.anchoredPosition.y);
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


