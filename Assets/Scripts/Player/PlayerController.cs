using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float forwardSpeed = 10f; // Velocidad de avance en Z
    public float blockSize = 5f; // Tamaño del bloque en X (carriles)
    public int maxLanes = 3; // Número máximo de carriles

    private float targetX; // Posición objetivo en X
    private float minX, maxX; // Límites en X
    private bool gameStarted = false; // Bandera para iniciar el movimiento

    void Start()
    {
        // Posición inicial en el carril central
        targetX = 0; // Si hay 3 carriles: (-1, 0, +1)
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        // Configurar los límites de X basados en los carriles
        minX = -((maxLanes - 1) / 2f) * blockSize; // Primer carril
        maxX = ((maxLanes - 1) / 2f) * blockSize; // Último carril
    }


    void Update()
    {
        // Esperar clic para empezar a moverse
        if (!gameStarted && Input.GetMouseButtonDown(0))
        {
            gameStarted = true;
        }

        // Si el juego ha empezado, avanzar automáticamente en Z
        if (gameStarted)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        // Movimiento lateral (X) con W y S
        if (Input.GetKeyDown(KeyCode.W) && targetX - blockSize >= minX)
        {
            targetX -= blockSize; // Moverse a la izquierda
        }
        else if (Input.GetKeyDown(KeyCode.S) && targetX + blockSize <= maxX)
        {
            targetX += blockSize; // Moverse a la derecha
        }

        // Aplicar el movimiento lateral con suavidad
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }
}

