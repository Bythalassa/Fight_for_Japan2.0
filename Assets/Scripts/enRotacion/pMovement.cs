using UnityEngine;

public class pMovement : MonoBehaviour
{
    public float speed;
    public Vector2 MoveDir;

    // Facing real del player: guarda la ultima direccion NO-CERO del input.
    // Si el player suelta las teclas, se queda mirando hacia donde miraba
    // por ultima vez (no vuelve a Vector2.zero), que es el comportamiento
    // esperado para "hacia donde mira" en un top-down.
    public Vector2 FacingDirection { get; private set; } = Vector2.down; //falta mucho testeo de esto

    // True solo en los frames en los que hay input real (para DummyMovement.IsPlayerMoving()).


    void Start()
    {


    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        MoveDir = new Vector2(x, y).normalized;    

        Vector3 direction = new Vector3(x, y, 0);
        direction.Normalize();

        if (direction.sqrMagnitude > 0.0001f)
        {
            FacingDirection = new Vector2(direction.x, direction.y);
        }

        transform.position += direction * speed * Time.deltaTime;
    }
}