using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
  //  Attack,
    BasicVaiven, 
    Dead,
}

public class MovimientoEnemiesLobby : MonoBehaviour
{

    public Rigidbody2D rb;

    public EnemyEnum state = EnemyEnum.Idle;
    public Health scriptHealth;
    public float Speed;
    public float ChaseSpeed;

    public float damage;
    public float radiusMovement; 
    public float radiusAttack;

    public bool isAbleToAttack = true;
    /*private float currentTime;
    private float MaxTime = 2f;*/

    //IDDLE
    private Vector2[] IdleOffsetsX = new Vector2[] {
    new Vector2(-3.5f, 0),   // izquierda: se resta a basePos
    new Vector2(3.5f, 0)   // derecha: se suma a basePos
}; //Offsets (Desplazamientos / Márgenes)

    private Vector3 basePos;   // posicion inicial de referencia
    private int targetIndex = 0; // hacia que punto se esta moviendo

    //BasicVaiven
    private Vector2[] VaivenOffsetsXY = new Vector2[] {
    new Vector2(2.5f, 1.9f),  // paso 1 (magnitudes, sin signo)
    new Vector2(3f, 1.2f)      // paso 2 (magnitudes, sin signo)


}; //Offsets (Desplazamientos / Márgenes)
    private float side;
    private float minDistance = 0.1f;
    private bool sideCalculated = false;
    public float VaiSpeed = 9;


    public float DetectionRadiusOne;//7f bastante para que lo persiga al toque 
    public float DetectionRadiusTwo;// 1.5f poco para que sea un movimiento exclusivo
    public float exitBuffer = 1.5f;
    public float exitBufferVaiven = 0.3f;


  /*  [Header("Pacing 4 attacking --> applies to WaitPhase & Camping")]
    [SerializeField] private int vueltasParaAtacar = 3;*/



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
       // arreglar esto de dead pero es solo para anim asiq big F if (scriptHealth.Vida <= 0) { state = EnemyEnum.Dead; }

        if (Relevo.CurrentPlayer == null) return; // por si el enemigo se activa antes que Relevo
        //Debug.Log("CurrentPlayer is " + Relevo.CurrentPlayer.name);

        Vector3 PlayerTargetPos = Relevo.CurrentPlayer.position;
        Vector3 myPos = transform.position;

        if (Vector3.Distance(PlayerTargetPos, myPos) > DetectionRadiusOne + exitBuffer)
        {
            state = EnemyEnum.Idle;
        }
        else if (Vector3.Distance(PlayerTargetPos, myPos) <= DetectionRadiusTwo)
        {
            state = EnemyEnum.BasicVaiven;
        }
        else if (Vector3.Distance(PlayerTargetPos, myPos) > DetectionRadiusTwo + exitBufferVaiven && Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadiusOne)
        {
            state = EnemyEnum.Chase;
        }


        switch (state)
        {
            case EnemyEnum.None:
                break;
            case EnemyEnum.Idle: //A.K A Wait phase. reduced version
                {
                    Debug.Log("Distance(PlayerTargetPos, myPos) > DetectionRadiusOne) : Enemy is in Idle");
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
                }
                break;
            case EnemyEnum.Chase:
                {
                    Debug.Log("(Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadiusOne) : Enemy is in Chase");

                    Vector3 direction = (PlayerTargetPos - myPos).normalized;
                    Vector2 newPos = rb.position + (Vector2)(direction * ChaseSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);

                }
                break;
            case EnemyEnum.BasicVaiven:
                {
                    Debug.Log("Distance(PlayerTargetPos, myPos) <= DetectionRadiusTwo) : Enemy is in BasicVaiven");

                    //solo en el estado de basic vaiven puede irse a estado -> attack 
                    //comportamiento esperado contexto: 
                    /*1// comportamiento esperado de BasicVaiven  : hace lo mismo que hace idle 
                    pero esta vez calcula posiciones actualizadas del player 
                    // -> dentro del rango de ATTACK RADIUS va a sumar o restar su posicion constantemente en ambos ejes x, y 
                    // de forma que logra hacer una diagonal 
                    */

                    //parte 1 movimiento esperado : 
                    /*Solo entra a Basic Vaiven si esta en DetectionRadiusTwo o menos
                     * actualiza su BasePos -> suma basePos.x && basePos.y a los intervalos nuevos (1 arriba - izquierda 2. abajo - derecha)
                     */

                    // side se calcula UNA sola vez por ciclo, no cada frame
                    if (!sideCalculated)
                    {
                        side = (myPos.x < PlayerTargetPos.x) ? -1f : 1f;
                        sideCalculated = true;
                    }

                    // ancla: sigue al player, manteniendo 0.5f de distancia minima en X
                    Vector3 anchor = PlayerTargetPos + new Vector3(side * minDistance, 0, 0);

                    Vector3 targetPos = new Vector3(
                        anchor.x + side * VaivenOffsetsXY[targetIndex].x,
                        anchor.y + side * VaivenOffsetsXY[targetIndex].y,
                        anchor.z
                    );

                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, VaiSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);

                    if (Vector2.Distance(rb.position, targetPos) < 0.01f)
                    {
                        targetIndex = (targetIndex + 1) % VaivenOffsetsXY.Length;
                        if (targetIndex == 0) sideCalculated = false;
                    }

                    /*parte 2:cambia de estado a ataque
                     * calcula su posicion 3 veces
                     * en el indice 3 -> case switch to attack => en attack se repite el movimiento vaiven 
                     * -> no se lopeea solo pasa 1 vez ya que al llegar al punto más cercano del player lo ataca
                     * -> luego de take damage -> regresa al estado de BasicVaiven o otros estados              
                     */

                    // y va a hacer daño porque yolo 

                    Debug.Log("Atacando");

                    if (isAbleToAttack)
                    {
                        Debug.Log("Atacando");
                        Relevo.CurrentPlayer.GetComponent<Health>().TakeDamage(damage);
                        isAbleToAttack = false;
                    }

                    Relevo.CurrentPlayer.GetComponent<Health>().Vida -= damage;

                }
                break;
           /* case EnemyEnum.Attack:
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
                }
                break;*/
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
