using JetBrains.Annotations;
using UnityEngine;

public class EnemyMovLobbyDos : MonoBehaviour
{
    public enum EnemyEnum
    {
        None,
        Idle,
        Chase,
        Attack,
        Dead,
    }

    public Rigidbody2D rb;

    public EnemyEnum state = EnemyEnum.Idle;
    public Health scriptHealth;
    public float Speed;
    public float ChaseSpeed;

    public float damage;
    public float radiusAttack;
    public float DetectionRadiusOne = 7f;//7f bastante para que lo persiga al toque 


    public bool isAbleToAttack = true;

    //IDDLE
    private Vector2[] IdleOffsetsX = new Vector2[] {
    new Vector2(-3.5f, 0),   // izquierda: se resta a basePos
    new Vector2(3.5f, 0)   // derecha: se suma a basePos
}; //Offsets (Desplazamientos / Márgenes)
    private Vector3 basePos;   // posicion inicial de referencia
    private int targetIndex = 0; // hacia que punto se esta moviendo

    //Cooldown Ataque
    public bool AtaqueEnemigoDisponible = false;
    public float TiempoActual;
    public float TiempoMaximoAtaque = 2f;



    void Start()
    {
        basePos = transform.position;
        Debug.Log("Enemy is in position" + basePos);
    }

    // Update is called once per frame
    void Update()
    {

        if (Relevo.CurrentPlayer == null) return;

        Vector3 PlayerTargetPos = Relevo.CurrentPlayer.position;
        Vector3 myPos = transform.position;

        EstadoEnemigo();
        TiempoAtaqueEnemigo();

        switch (state)
        {
            case EnemyEnum.None:
                break;
            case EnemyEnum.Idle: //A.K A Wait phase. reduced version
                {
                    Debug.Log("Distance(PlayerTargetPos, myPos) > DetectionRadiusOne) : Enemy is in Idle");

                    Vector3 targetPos = new Vector3(basePos.x + IdleOffsetsX[targetIndex].x, basePos.y, basePos.z);
                    Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, Speed * Time.deltaTime);

                    rb.MovePosition(newPos);
                    //-> MovePosition es una función de movimiento

                    if (Vector2.Distance(rb.position, targetPos) < 0.01f)
                    {
                        targetIndex = (targetIndex + 1) % IdleOffsetsX.Length;
                    }

                    if (Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadiusOne) { state = EnemyEnum.Chase; }
                    if (Vector3.Distance(PlayerTargetPos, myPos) < radiusAttack) { state = EnemyEnum.Attack; }
                }
                break;
            case EnemyEnum.Chase:
                {
                    Debug.Log("(Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadiusOne) : Enemy is in Chase");

                    Vector3 direction = (PlayerTargetPos - myPos).normalized;
                    Vector2 newPos = rb.position + (Vector2)(direction * ChaseSpeed * Time.deltaTime);
                    rb.MovePosition(newPos);

                    if (Vector3.Distance(PlayerTargetPos, myPos) > DetectionRadiusOne) { state = EnemyEnum.Idle; } ;

                    if (Vector3.Distance(PlayerTargetPos, myPos) < radiusAttack) { state = EnemyEnum.Attack; }    

                }
                break;
            case EnemyEnum.Attack:
                {
                    Debug.Log("Atacando");

                    if (AtaqueEnemigoDisponible)
                    {
                        Relevo.CurrentPlayer.GetComponent<Health>().TakeDamage(2);//.Vida -= damage;
                        AtaqueEnemigoDisponible = false;
                    }

                    if (Vector3.Distance(PlayerTargetPos, myPos) < DetectionRadiusOne) { state = EnemyEnum.Chase; }

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

    public void TiempoAtaqueEnemigo()
    {
        TiempoActual += Time.deltaTime;
        if(TiempoActual >= TiempoMaximoAtaque)
        {
            AtaqueEnemigoDisponible = true;
            TiempoActual = 0;
        }
    }

    public void EstadoEnemigo()
    {
        if (!AtaqueEnemigoDisponible)
        {
            TiempoAtaqueEnemigo();
        }
    }




}
