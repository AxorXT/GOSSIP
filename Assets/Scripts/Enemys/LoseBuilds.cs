using UnityEngine;

public class LoseBuilds : MonoBehaviour
{
    public GameObject ScreenLose; 
    public GameObject Player;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado con: " + other.gameObject.name); // Para depuración

        if (other.CompareTag("Player"))
        {
            Debug.Log("Colisión con edificio, mostrando pantalla de muerte...");
            Player.SetActive(false);
            ScreenLose.SetActive(true);
        }
    }
}
