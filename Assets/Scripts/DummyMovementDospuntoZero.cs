using UnityEngine;

public enum DummyState
{
    IdleAfterSpawn,
    WalkBehind,
    Approach,
    WaitPhase,
    Camping,
    CrossRange,
    ExtremeSway,
    Recovery,
    Attacking,
    Dead
}

public class DummyMovementDospuntoZero : MonoBehaviour
{
    private GameObject playerTarget;
    public GameObject enemyTarget;
    public Animator anim;
    public Health scriptHealth;
    public EnemyCameraCheck scriptCamera;
    public pMovement scriptPMovement;

    [Header("Movimiento general")]
    public float Maxspeed = 3f;
    public float radiusMovement = 3f; //(medir que chota con esto pq wtf)
    public float AttackRadious = 2.5f;
    public float PassiveAttackRadious = 5f; //relativamente muy lejos del jugador
    public float sideOffset = 20f; //same here
    private float side; // -1 izquierda / 1 derecha respecto al player

    //var para la funcion de detectar camara &&  IdleAfterSpawn case
    private bool isOnCamRange = false;

    //lista de Changeable Vars je nach case context
    //TimetoIdle
    private float speed;//testear si puedo usar este speed para todo ekisdfeeee
    private float currentTime;
    private float MaxTime = 3f;


    [SerializeField] private RuntimeAnimatorController controladorThisEnemy; //no estoy usando esto

    [Header("Estado actual (solo lectura, para debug)")]
    public DummyState state = DummyState.IdleAfterSpawn;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        enemyTarget = GameObject.FindGameObjectWithTag("Enemy");
        anim = GetComponent<Animator>();
        scriptHealth = GetComponent<Health>();
        scriptCamera = GetComponent<EnemyCameraCheck>();
        scriptPMovement = GetComponent<pMovement>();
    }

    void Update()
    {

        //main updated position
        Vector3 PlayerTargetPos = playerTarget.transform.position;
        Vector3 EnemyTargetPos = enemyTarget.transform.position;
        Vector3 myPos = transform.position;

        Vector2 dirHaciaEnemigo = (myPos - PlayerTargetPos).normalized; //para lógica de detección (saber dónde está el enemigo respecto al jugador).
        Vector3 direction = (PlayerTargetPos - myPos).normalized; //para lógica de persecución (hacer que el enemigo avance hacia el jugador).
        float relacionMirada = Vector2.Dot(scriptPMovement.FacingDirection, dirHaciaEnemigo);

        CheckingVidavar();

        switch (state)
        {

            case DummyState.IdleAfterSpawn:
                transform.position += direction * Maxspeed * Time.deltaTime;

                if (isOnCamRange)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= MaxTime)
                    {
                        speed = 0f; // ya no c mueve
                        Maxspeed = speed;
                        anim.SetTrigger("Idle");

                        currentTime = 0;
                    }
                    else if (currentTime == 0) { state = DummyState.Approach; }
                }
                break;

            case DummyState.Approach:
                {
                    //allEnemy aproach it 
                    transform.position += direction * Maxspeed * Time.deltaTime;

                    if (relacionMirada > 0.5f)
                    {
                        state = DummyState.Camping;
                    }
                    else
                    {
                        state = DummyState.CrossRange;
                    }


                    /*condiciones extracurrriculares
                     * Caminar detras del player
                     * if despues de 1.5f segundos 
                     * 
                     * { if (!enemyTarget.transform.position = x.1.39366,z ) {  state = DummyState.WalkBehind; }
                     * else  transform.position += direction * Maxspeed * Time.deltaTime; 
                     * 
                     * if (5 EnemyTargetPos == AttackRadiousPassive ) {  state = DummyState.ExtremeSway; }
                     * lo mueve ligeramente lejos al player
                     * 
                     * if (3 EnemyTargetPos == AttackRadious ) { state = DummyState.WaitPhase;  }
                     * 
                    */

                }
                break;
            case DummyState.WalkBehind:
                {
                    //  if (enemyTarget.transform.position = x.1.39366,z ) { state = DummyState.Approach; }
                }
                break;
            case DummyState.WaitPhase:
                {
                    //que se mueva en campeo 
                    //en direccion -> alejandose de player = camina
                    //llega a posicion del player de vuelta = puede atacar = i (si i = 3 {case switch to attack y i= 0 })
                    //
                }
                break;
            case DummyState.Camping:
                {

                    //que se mueva en campeo 
                    //en direccion -> alejandose de player = camina
                    //llega a posicion del player de vuelta = puede atacar = i (si i = 3 {case switch to attack y i= 0 })
                    //If dentro de campeo is facing player 
                    //if (3 EnemyTargetPos == AttackRadious ) { state = DummyState.WaitPhase; }
                }
                break;
            case DummyState.CrossRange:
                {

                    //logica de cambiio
                    //y if player near : { state = DummyState.WaitPhase; }
                    //y if player LongDistance : { state = DummyState.Campeo; }


                }
                break;
            case DummyState.ExtremeSway:
                {

                }
                break;
            case DummyState.Recovery:
                {



                }
                break;
            case DummyState.Attacking:
                {
                    //solo animAtaque -> y Do damage . take from Vida 
                    //
                    //if player moving in y =  case switch campeo 
                    //if player moving in x =  case switch wait phase  
                }
                break;
            case DummyState.Dead:
                {

                }
                break;
            default:
                break;


               
        }
    } 



    private void CheckingVidavar()
    {
        if (scriptHealth.Vida <= scriptHealth.recoveryThreshold) { state = DummyState.Recovery; }
        if (scriptHealth.Vida <= scriptHealth.maxVida) { state = DummyState.Dead; }
    }

    

 }
