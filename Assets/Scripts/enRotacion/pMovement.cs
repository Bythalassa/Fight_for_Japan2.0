using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]


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
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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
      
        if (MoveDir.sqrMagnitude > 0.0001f)
        {
            FacingDirection = MoveDir;
        }
    }

    //El método FixedUpdate es una función especial que controla la fisica de Rigidbody
    void FixedUpdate()
    {
        rb.linearVelocity = MoveDir * speed;
        // rb.MovePosition(rb.position + MoveDir * speed * Time.fixedDeltaTime); // Rigidbody Dynamic
    }
}