using UnityEngine;
using UnityEngine.SceneManagement;

public class Lose : MonoBehaviour
{
    public GameObject ScreenLose;
    public GameObject Player;
    public GameObject ScoreScreen;


    public void Start()
    {
        ScreenLose.SetActive(false);
    }
    
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger detectado con: " + other.gameObject.name); // Para depuración

        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador detectado, desactivando...");
            Player.SetActive(false);
            ScreenLose.SetActive(true);
            ScoreScreen.SetActive(false);
        }
    }


}
