using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator enemyAnimator; // El Animator del enemigo
    private bool gameStarted = false;

    void Start()
    {
        // Configuraci�n inicial de las animaciones
        enemyAnimator.SetBool("IsIdle", true); // El enemigo est� quieto
        enemyAnimator.SetBool("IsWalking", false); // El enemigo no est� caminando
    }

    void Update()
    {
        // Cambiar a animaci�n de caminar cuando el juego empiece
        if (gameStarted)
        {
            enemyAnimator.SetBool("IsIdle", false);
            enemyAnimator.SetBool("IsWalking", true);
        }
    }
}
