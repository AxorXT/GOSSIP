using UnityEngine;

public class Lose : MonoBehaviour
{
    public GameObject ScreenLose;
    public GameObject Player;


    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado con: " + other.gameObject.name); // Para depuraci�n

        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado, desactivando...");
            Player.SetActive(false);
            ScreenLose.SetActive(true);
        }
    }

}
