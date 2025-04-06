using UnityEngine;

public class AnimacionesUI : MonoBehaviour
{

    [SerializeField] private GameObject GameName;
 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RectTransform rt = GameName.GetComponent<RectTransform>();

        // Primero posicionarlo fuera de la pantalla (izquierda)
        rt.anchoredPosition = new Vector2(-1000f, rt.anchoredPosition.y);

        // Animarlo hacia el centro (x = 0)
        LeanTween.move(rt, new Vector2(-5f, rt.anchoredPosition.y), 1.5f).setEaseOutExpo();
    }
   
    // Update is called once per frame

}
