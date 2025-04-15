using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InvisibleButtonHandler : MonoBehaviour, IPointerDownHandler
{
    public GameObject gameController; // Asocia tu controlador de juego aqu�, para iniciar el juego.

    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameController != null)
        {
            gameController.GetComponent<PlayerController>().StartGame(); // Llama a StartGame desde PlayerController
        }
    }
}
