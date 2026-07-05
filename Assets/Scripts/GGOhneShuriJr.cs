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

    public float radiusMovement = 3f ; // stop moving towards player
    public float sideOffset = 3.5f; // "Offset" es un desplazamiento relativo al eje del player
    
    //logica del vaivén
    public float hoverRange = 2.5f; // amplitud del vaivén(acercamiento/retroceso)
    public float hoverSpeed = 2f;   // velocidad del vaivén
    private float hoverOffset = 0f;
    private int hoverDirection = 5; // 1 = derecha, -1 = izquierda
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
                        state = DummyEnum.Near;
                }
                break;

            case DummyEnum.Near:
                {
                    // izquierda(sideOffset negativo) / derecha(sideOffset positivo)
                    float side = (myPos.x >= targetPos.x) ? -1f : 1f;
                    Vector3 sidePos = targetPos + new Vector3(sideOffset * side, 0f, 0f);
      
                    if (Vector3.Distance(myPos, sidePos) > 0.05f) //ignore player if in radious and hasn´t been attacked
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

                        transform.position = sidePos + new Vector3(hoverOffset, 0f, 0f);
                    }
                }
                break;

            default:
                break;
        }
    }
}