using UnityEngine;


//basic: 3 estados basicos (chase si no esta on pelea, si esta onpelea idle, attack si esta en el radio )
//en el radio un vaiven minimo 
//añadir anim de ataques a los 3 enemigos usar lo de cada uno maneja su animator controller
//do damage // health / morir
//requerimientos del otro script que siempre tiene que usar xra otras funciones 


public enum EnemyEnum
{
    None,
    Idle,
    Chase,
    Attack,
    BasicVaiven, 
    Dead,
}

public class MovimientoEnemiesLobby : MonoBehaviour
{
    //public GameObject PlayerTarget;
    public EnemyEnum state = EnemyEnum.Idle;
    public Health scriptHealth;
    public float Speed;

    public float damage;

    public float DetectionRadius;
    public float radiusMovement;
    public float radiusAttack;

    public bool isAbleToAttack = true;
    private float currentTime;
    private float MaxTime = 2f;

    private Vector2[] IdleOffsetsX = new Vector2[] {
                    new Vector2(12.26f, 0), // punto izquierda
                    new Vector2(16.15f, 0) };  // punto derecha                  
                    //Offsets (Desplazamientos / Márgenes)

    private Vector3 basePos;   // posicion inicial de referencia
    private int targetIndex = 0; // hacia que punto se esta moviendo


    [Header("Pacing 4 attacking --> applies to WaitPhase & Camping")]
    [SerializeField] private int vueltasParaAtacar = 3;






    /*recreation of Camping state*/
    //camping config
    private Vector2[] campingOffsetsIzquierda = new Vector2[] {
    new Vector2(2.37f, 1.90f),
    new Vector2(2f, 1.20f) };
    private Vector2[] campingOffsetsDerecha = new Vector2[] {
    new Vector2(2.37f, 1.90f),
    new Vector2(2f, 1.20f) };
    private int vueltaCount = 0;
    private float previousSide = 0f;
    private int waypointIndex = 0;
    private float side; // -1 izquierda / 1 derecha respecto al player




    void Start()
    {
        //--> iddle / wait case 
        basePos = transform.position;
        Debug.Log("Enemy is in position" + basePos);
        /*Enemy is in position(10.91, -1.43, 0.00)*/
        
    }

    // Update is called once per frame
    void Update()
    {


        if (Relevo.CurrentPlayer == null) return; // por si el enemigo se activa antes que Relevo
        Debug.Log("CurrentPlayer is " + Relevo.CurrentPlayer.name);

        Vector3 PlayerTargetPos = Relevo.CurrentPlayer.position;
        Vector3 myPos = transform.position;

        /*if (scriptHealth != null && scriptHealth.Vida <= 0 && state != EnemyEnum.Dead)
        {
            state = EnemyEnum.Dead;
        }*/

        switch (state)
        {
            case EnemyEnum.None:
                break;
            case EnemyEnum.Idle: //A.K A Wait phase. reduced version
                {
                    Debug.Log("Enemy is in Idle");
                    //movimiento esperado : moverse hacia adelante y hacia atras en vaiven lineal
                    //valores de movimiento: 

                    Vector3 myNewPos = basePos + (Vector3)IdleOffsetsX[targetIndex];
                    Debug.Log("Enemy is in position -> targetPos = basePos + (Vector3)IdleOffsetsX[targetIndex] is : " + myNewPos);
                    /*Enemy is in position Enemy is in position -> targetPos = basePos + (Vector3)IdleOffsetsX[targetIndex] is : (23.17, -1.43, 0.00)*/
                    //el vector x se asigno 23.17  pero no tiene sentido porque su movimiento no se suma sino se resta ()
                    // entonces la pregunta es cual es la mate que asigna el vector para que se reste ligeramente a la posicion actual ejemplo -2f y luego
                    //se sume 2.5f constantemente en un loop de movimiento izquierda a derecha

                    transform.position = Vector3.MoveTowards(transform.position, PlayerTargetPos, Speed * Time.deltaTime);
                    

                    if (Vector3.Distance(transform.position, myNewPos) < 0.01f)
                        Debug.Log("Distance is minor than 0.01f ");
                    //olvidate, la mate para pasar a derecha es si se resto su posición (izquierda) o se se sumo su posicion (derecha) 

                    {
                        targetIndex = (targetIndex + 1) % IdleOffsetsX.Length;
                    }


                    /*
                    Vector3 newTargetPos = (side == -1f)
                            ? new Vector3(myPos.x - 2f, transform.position.y, transform.position.z)
                            : new Vector3(myPos.x + 2f, transform.position.y, transform.position.z);

                    float distanceBeforeMove = Vector3.Distance(PlayerTargetPos, myPos);
                    //infos for debug:
                    //Calcula la distancia en línea recta entre la posición del jugador y la del enemigo en el espacio 3D.

                    if (distanceBeforeMove <= radiusMovement)
                        //infos for debug:
                        //Sabiendo que radiusMovement del enemigo del inspector actual es 10f
                        //la distancia entre player y enemy es menor a 10f entonces ->

                        Debug.Log($"distanceBeforeMove is {distanceBeforeMove}"); //rpta : 6.32267 
                        // las vars van en {}  { distanceBeforeMove}
                    {
                        side = (myPos.x >= PlayerTargetPos.x) ? -1f : 1f;
                        //float side evalua si la x esta en -1f :1f

                        // si el lado cambió respecto al anterior, se completó una vuelta
                        if (side != previousSide && previousSide != 0f) { vueltaCount++; }
                        previousSide = side;
                    }

                    //aqui recien lo muevo a NewTarget pos
                    transform.position = Vector3.MoveTowards(transform.position, newTargetPos, Speed * Time.deltaTime);
                    //debugear hacia donde se esta moviendo


                    //comparando esta logica con el unico vaiven que si sirve en el script> 
                    //arriba de start 
                    /* private Vector2[] campingOffsetsUp = new Vector2[] {
                             new Vector2(0, 1.26f), // punto arriba
                             new Vector2(0, 0.15f)  // punto abajo
                         };

                         private Vector3 basePos;   // posicion inicial de referencia
                         private int targetIndex = 0; // hacia que punto se esta moviendo

                      //en start 
                        basePos = transform.position;

                    //la funcion es solo 3 lineas ptm 
                    Vector3 targetPos = basePos + (Vector3)campingOffsetsUp[targetIndex];
                    
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, Speed * Time.deltaTime);

                        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                        {
                            targetIndex = (targetIndex + 1) % campingOffsetsUp.Length;
                        }

                     */


                    if (Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadius)
                        state = EnemyEnum.Chase;
                }
                break;
            case EnemyEnum.Chase:
                {
                    Debug.Log("Enemy is in Chase");
                    Vector3 direction = (PlayerTargetPos - myPos).normalized;
                    transform.position += direction * Speed * Time.deltaTime;

                    if (Vector3.Distance(PlayerTargetPos, myPos) > DetectionRadius)
                        state = EnemyEnum.Idle;
                    if (Vector3.Distance(PlayerTargetPos, myPos) < radiusAttack)
                        state = EnemyEnum.BasicVaiven;
                }
                break;
            case EnemyEnum.BasicVaiven:
                {
                    // intentando recrear el bloque de Camping del script nivel 2 

                    float distanceBeforeMove = Vector3.Distance(PlayerTargetPos, myPos);
                    Vector2 offsetIzq = campingOffsetsIzquierda[waypointIndex];
                    Vector2 offsetDer = campingOffsetsDerecha[waypointIndex];

                    if (distanceBeforeMove <= radiusMovement)
                    {
                        side = (myPos.x >= PlayerTargetPos.x) ? -1f : 1f;

                        if (side != previousSide && previousSide != 0f)
                        {
                            vueltaCount++;
                        }
                        previousSide = side;
                    }

                    // si x = -1 (izquierda), se resta myPos - offset
                    Vector3 newTargetPosIzquierda = new Vector3(
                        myPos.x - offsetIzq.x,
                        myPos.y - offsetIzq.y,
                        transform.position.z
                    );

                    // si x = 1 (derecha), se suma myPos + offset
                    Vector3 newTargetPosDerecha = new Vector3(
                        myPos.x + offsetDer.x,
                        myPos.y + offsetDer.y,
                        transform.position.z
                    );

                    // ambos ejes se reflejan según el lado, y se calculan respecto a myPos, no acumulando -> segun la posicion en x, y se ejecuta la matematica
                    // aqui necesitaria otro vector que calcula posiciones en resta si myPos x and y 
                    // primer      calculo si x = -1 se restara en la siguiente posicion --2.37 , y = -1 se restara en la siguiente posicion -1.90
                    // segundo calculo se calcula myPos.x restandolo -2 , y = myPos.y restandolo -1.20
                    // se ejecuta el primer calculo de nuevo 
                    // me imagino que deberia habre un newTargetPosIzquierda y newTargetPosDerecha

                    /* el otro vector
                     * primer calculo si x = 1 se sumara en la siguiente posicion +2.37 , y = +1 se sumara en la siguiente posicion +1.90
                    // segundo calculo se calcula myPos.x sumandolo +2 , y = myPos.y sumandolo +1.20
                    // se ejecuta el primer calculo de nuevo 
                    // me imagino que deberia habre un newTargetPosIzquierda y newTargetPosDerecha                     
                     */

                    Vector3 newTargetPos = (side == -1f) ? newTargetPosIzquierda : newTargetPosDerecha;

                    transform.position = Vector3.MoveTowards(transform.position, newTargetPos, Speed * Time.deltaTime);

                    if (Vector3.Distance(transform.position, newTargetPos) < 0.05f)
                    {
                        waypointIndex = (waypointIndex + 1) % campingOffsetsIzquierda.Length;
                        // avanzando el índice usando campingOffsetsIzquierda.Length como referencia de
                        // "cuántos pasos tiene el ciclo" — asumiendo que ambos arrays tienen el mismo largo
                    }

                    if (vueltaCount >= vueltasParaAtacar)
                    {
                        float distanceAfterMove = Vector3.Distance(PlayerTargetPos, transform.position);
                        if (distanceAfterMove < distanceBeforeMove)
                        {
                            state = EnemyEnum.Attack;
                            vueltaCount = 0;
                            waypointIndex = 0;
                        }
                    }

                    if (scriptHealth.Vida <= 0) { state = EnemyEnum.Dead; }

                    //MOVIMIENTO VAIVEN 
                    //INDEX x 3 veces
                    //3ra vez -> case switch : ataca
                    //se resetea
                    //si ataco 1 vez case swithc to vaivem

                }
                break;
            case EnemyEnum.Attack:
                {
                    if (isAbleToAttack)
                    {
                        Debug.Log("Atacando");
                        Relevo.CurrentPlayer.GetComponent<Health>().TakeDamage(damage);
                        isAbleToAttack = false;
                    }
                    currentTime += Time.deltaTime;
                    if (currentTime >= MaxTime)
                    {
                        isAbleToAttack = true;

                        currentTime = 0;
                    }

                    if (Vector3.Distance(PlayerTargetPos, myPos) > radiusAttack)
                        state = EnemyEnum.Chase;
                }
                break;
            case EnemyEnum.Dead:
                {
                    //SetAnim("Death");
                    Destroy(gameObject);
                }
                break;
                    default:
                break;
        }
    }
}
