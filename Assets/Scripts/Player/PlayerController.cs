using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator playerAnimator; // El Animator del jugador
    public float forwardSpeed = 10f; // Velocidad del jugador
    private bool gameStarted = false; // Estado del juego
    private float targetX; // Posici�n objetivo en X
    public float blockSize = 5f; // Tama�o del bloque
    public int maxLanes = 3; // N�mero de carriles
    private float minX, maxX;

    void Start()
    {
        // Configuraci�n inicial de las animaciones
        playerAnimator.SetBool("IsIdle", true); // El jugador est� quieto
        playerAnimator.SetBool("IsWalking", false); // El jugador no est� caminando

        // Posici�n inicial
        targetX = 0;
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        minX = -((maxLanes - 1) / 2f) * blockSize;
        maxX = ((maxLanes - 1) / 2f) * blockSize;
    }

    void Update()
    {
        // Esperar clic para comenzar el juego
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            gameStarted = true;
            playerAnimator.SetBool("IsIdle", false); // Detener animaci�n de respiraci�n
            playerAnimator.SetBool("IsWalking", true); // Iniciar animaci�n de caminar
        }

        // Movimiento lateral y avance
        if (gameStarted)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        // Movimiento lateral con W y S
        if (Input.GetKeyDown(KeyCode.W) && targetX - blockSize >= minX)
        {
            targetX -= blockSize;
        }
        else if (Input.GetKeyDown(KeyCode.S) && targetX + blockSize <= maxX)
        {
            targetX += blockSize;
        }

        // Aplicar suavemente el movimiento lateral
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }

 
}

