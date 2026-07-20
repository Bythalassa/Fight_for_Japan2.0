using UnityEngine;

public class CompitaManager : MonoBehaviour
{

    public bool IsMoving { get; private set; }

    private Animator animator;
    public pMovement jugador;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (jugador.MoveDir != Vector2.zero)
        {
            animator.SetFloat("X", jugador.MoveDir.x);
            animator.SetFloat("Y", jugador.MoveDir.y);

            animator.SetBool("IsMoving", true); //->SetBool(string name, bool value)
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

    }
}
