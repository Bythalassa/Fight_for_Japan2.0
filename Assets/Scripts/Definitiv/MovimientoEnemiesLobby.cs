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
    public Rigidbody2D rb;

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
    new Vector2(-3.5f, 0),   // izquierda: se resta a basePos
    new Vector2(3.5f, 0)   // derecha: se suma a basePos
};
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
        /*Enemy is in position(xx)*/
        
    }

    // Update is called once per frame
    void Update()
    {

        if (scriptHealth.Vida <= 0) { state = EnemyEnum.Dead; }
        if (Relevo.CurrentPlayer == null) return; // por si el enemigo se activa antes que Relevo
        //Debug.Log("CurrentPlayer is " + Relevo.CurrentPlayer.name);

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
                    //parte 1 movimiento esperado : vaiven lineal
                    /*el comportamiento deberia ser, aunque mi player se mueve 
                     * el enemigo se queda en el movimiento izquierda a derecha }
                     * hasta que el radio de deteccion se hace menor solo porque el player se acerca */
                    //valores de movimiento: 

                    /*parte 2: 
                     FixedUpdate()Física, Rigidbody, rb.MovePosition()
                     */


                    Vector3 targetPos = new Vector3(basePos.x + IdleOffsetsX[targetIndex].x, basePos.y, basePos.z);
                    // la mate que suma la posición de base con los margenes

                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, Speed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    //-> MovePosition es una función de movimiento

                    if (Vector2.Distance(rb.position, targetPos) < 0.01f)
                    {
                        targetIndex = (targetIndex + 1) % IdleOffsetsX.Length;
                    }

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
                    Debug.Log("Enemy is in BasicVaiven");

                    //solo en el estado de basic vaiven puede irse a estado -> attack 
                    //comportamiento esperado contexto: 
                    /*1// comportamiento esperado de BasicVaiven  : hace lo mismo que hace idle 
                    pero esta vez calcula posiciones actualizadas del player 
                    // -> dentro del rango de ATTACK RADIUS va a sumar o restar su posicion constantemente en ambos ejes x, y 
                    // de forma que logra hacer una diagonal 
                    */

                    //parte 1 movimiento esperado : 
                    /*cc
                     * */

                    /*parte 2: 
                     cc
                     */


                    Vector3 targetPos = new Vector3(basePos.x + IdleOffsetsX[targetIndex].x, basePos.y, basePos.z);
                    // la mate que suma la posición de base con los margenes

                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, Speed * Time.deltaTime);
                    rb.MovePosition(newPos);
                    //-> MovePosition es una función de movimiento

                    if (Vector2.Distance(rb.position, targetPos) < 0.01f)
                    {
                        targetIndex = (targetIndex + 1) % IdleOffsetsX.Length;
                    }

                    if (Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadius)
                        state = EnemyEnum.Chase;



                }
                break;
            case EnemyEnum.Attack:
                {
                    Debug.Log("Enemy is in Attack");

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
                    //-> salida a idle
                    //-> salida a Basic vaiven 


                }
                break;
            case EnemyEnum.Dead:
                {
                    Debug.Log("Enemy is Dead");
                    //SetAnim("Death");
                }
                break;
                    default:
                break;
        }
    }
}
