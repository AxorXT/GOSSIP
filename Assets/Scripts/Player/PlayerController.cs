using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 10f;
    public float laneDistance = 2f;
    private int lane = 1; // 0 = izquierda, 1 = centro, 2 = derecha

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.LeftArrow) && lane > 0)
        {
            lane--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && lane < 10)
        {
            lane++;
        }

        Vector3 targetPosition = new Vector3((lane - 1) * laneDistance, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 10);
    }
}

