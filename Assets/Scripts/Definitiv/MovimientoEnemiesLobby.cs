using Unity.Multiplayer.PlayMode;
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
        /* PlayerTarget = GameObject.FindGameObjectWithTag("Player");
         Debug.Log("Player Target is" + PlayerTarget);*/

        //Debug.Log("CurrentPlayer is" + Relevo.CurrentPlayer);
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
            case EnemyEnum.Idle:
                {
                    if (Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadius)
                        state = EnemyEnum.Chase;
                }
                break;
            case EnemyEnum.Chase:
                {
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
                    // primer calculo si x = -1 se restara en la siguiente posicion --2.37 , y = -1 se restara en la siguiente posicion -1.90
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
