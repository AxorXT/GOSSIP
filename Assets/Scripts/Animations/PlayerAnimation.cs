using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private bool alreadyStarted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isWalking", false); // Empieza en Idle
    }

    void Update()
    {
        if (!alreadyStarted && PlayerController.GameStartedGlobally)
        {
            animator.SetBool("isWalking", true); // Cambia a Walking
            alreadyStarted = true;
        }
    }
}
