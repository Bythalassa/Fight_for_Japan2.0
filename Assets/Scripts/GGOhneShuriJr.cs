using UnityEngine;

public enum DummyEnum
{
    None,
    LongDistance,
    Near
}

public class DummyGGOhneShuriJr : MonoBehaviour
{
    public DummyEnum state = DummyEnum.LongDistance;
    private GameObject Target;
    public float Speed;
    public float Health;

    public float radiusMovement = 3f; // stop moving towards player
    public float sideOffset = 20f; //  la posición fija de destino / punto de corte

    //logica del vaivén
    private float side;
    public float hoverRange = 5.5f; // amplitud del vaivén(acercamiento/retroceso)
    public float hoverSpeed = 9f;   // velocidad del vaivén
    private float hoverOffset = 0f;
    private int hoverDirection = 1; // 1 = derecha, -1 = izquierda // no puede ser más q 1 (se queda en 1)
    public float minDistance = 5f; // distancia mínima relative 2 player

    //hover is equal to rondar/dar vueltas al rededor de un eje 

    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    { 
        Vector3 targetPos = Target.transform.position;
        Vector3 myPos = transform.position;

        switch (state)
        {
            case DummyEnum.None:
                break;

            case DummyEnum.LongDistance:
                {  
                    Vector3 direction = (targetPos - myPos).normalized;
                    transform.position += direction * Speed * Time.deltaTime;

                    if (Vector3.Distance(targetPos, myPos) <= radiusMovement)
                    {
                        side = (myPos.x >= targetPos.x) ? -1f : 1f; //al llegar al rango minimo de alcance, calcula la posición
                        Debug.Log("enemy distance is less than radiousMovement");
                        state = DummyEnum.Near;
                    }                        
                }
                break;

            case DummyEnum.Near:
                {
                    {
                        Vector3 sidePos = targetPos + new Vector3(sideOffset * side, 0f, 0f); /// side no tiene sentido en este contexto 
                        //calcula la posición lateral del enemigo en relacion al jugador, desplazandolo el valor de Offset en x

                        if (Vector3.Distance(myPos, sidePos) > 0.05f)
                        //si la distancia entre el enemigo y la posición lateral es mayor a 0.05,
                        //deberia desplezarse a sidePos 
                        {
                            Vector3 direction = (sidePos - myPos).normalized;
                            transform.position += direction * Speed * Time.deltaTime;
                        }
                        else
                        {
                            hoverOffset += hoverDirection * hoverSpeed * Time.deltaTime;
                            if (hoverOffset > hoverRange || hoverOffset < -hoverRange)
                            {
                                hoverDirection *= -1; // acercar / alejarse
                            }

                            float newX = sidePos.x + hoverOffset;

                            // evita que el vaivén cruce hacia el lado del player (nunca sobreponerse)
                            if (side > 0 && newX < targetPos.x + minDistance)
                                newX = targetPos.x + minDistance;
                            else if (side < 0 && newX > targetPos.x - minDistance)
                                newX = targetPos.x - minDistance;

                            transform.position = new Vector3(newX, sidePos.y, sidePos.z);
                        }
                    }
                }
                    break;
                default:
                break;
        }
    }
}