using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Referencia al jugador
    public Vector3 offset = new Vector3(0, 10, -10); // Distancia de la cámara al jugador
    public float smoothSpeed = 5f; // Suavidad del movimiento

    void LateUpdate()
    {
        if (player == null) return; // Asegurar que el jugador existe

        // Posición objetivo basada en el jugador + el offset
        Vector3 targetPosition = player.position + offset;

        // Interpolación suave para un movimiento fluido
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // Opcional: Mantener la rotación fija mirando hacia adelante
        transform.LookAt(player.position + Vector3.forward * 5);
    }
}
