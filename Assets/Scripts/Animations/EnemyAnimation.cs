using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator enemyAnimator; // El Animator del enemigo
    private bool gameStarted = false;

    void Start()
    {
        // Configuración inicial de las animaciones
        enemyAnimator.SetBool("IsIdle", true); // El enemigo está quieto
        enemyAnimator.SetBool("IsWalking", false); // El enemigo no está caminando
    }

    void Update()
    {
        // Cambiar a animación de caminar cuando el juego empiece
        if (gameStarted)
        {
            enemyAnimator.SetBool("IsIdle", false);
            enemyAnimator.SetBool("IsWalking", true);
        }
    }
}
