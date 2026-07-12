using UnityEngine;

public class MovimientoMonjeJose : MonoBehaviour
{
    private float Speed = 1.7f;
    public SpriteRenderer sombraSmall;
    public SpriteRenderer sombraBig;

    private Vector2[] campingOffsetsUp = new Vector2[] {
        new Vector2(0, 1.26f), // punto arriba
        new Vector2(0, 0.15f)  // punto abajo
    };
    //Offsets (Desplazamientos / Márgenes)
    //Up = Y

    private Vector3 basePos;   // posicion inicial de referencia
    private int targetIndex = 0; // hacia que punto se esta moviendo

    void Start()
    {
        basePos = transform.position;
        //1--el script guarda la posición inicial
    }

    void Update()
    {
        JMoves();
        ActualizarSombra();
    }

    private void JMoves()
    {
        // Punto objetivo actual (posicion base + offset)
        Vector3 targetPos = basePos + (Vector3)campingOffsetsUp[targetIndex];
        //(Vector3)campingOffsetsUp[targetIndex] es un cast (conversión de tipo):
        //le estás diciendo a C# "convierte este Vector2 en un Vector3
        //sin el cast no podria sumar el Vector 3 de movimiento con la matematica aplicada de Vectores2

        // Movernos hacia la posicón actualizada según el calculo x,y.
        transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);

        // Si llegamos, cambiamos de objetivo (arriba <-> abajo)
        //aqui la tengo que pensar un poco más
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            targetIndex = (targetIndex + 1) % campingOffsetsUp.Length;
        }
    }

    private void ActualizarSombra()
    {
        if (targetIndex == 0)
        {
            // se mueve hacia arriba
            sombraSmall.enabled = true;
            sombraBig.enabled = false;
        }
        else
        {
            // se mueve hacia abajo
            sombraSmall.enabled = false;
            sombraBig.enabled = true;
        }
    }

}