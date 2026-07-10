using System.Collections.Generic;
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
   // public Animator anim;
    public Health scriptHealth;
    public EnemyCameraCheck scriptCamera;
    public pMovement scriptPMovement;

    [Header("Movimiento general")]
    public float Maxspeed = 3f;
    public float radiusMovement = 3f; //(medir que chota con esto pq wtf)
    //public float AttackRadious = 2.5f; llamar al radiusAttack del script PlayerAttack.cs
    public float PassiveAttackRadious = 5f; //relativamente muy lejos del jugador
    public float sideOffset = 20f; //same here
    private float side; // -1 izquierda / 1 derecha respecto al player

    [Header("Pacing 4 attacking --> applies to WaitPhase & Camping")]
    [SerializeField] private int vueltasParaAtacar = 3;
   // [SerializeField] private RuntimeAnimatorController controladorThisEnemy; //no estoy usando esto

    [Header("Estado actual (solo lectura, para debug)")]
    public DummyState state = DummyState.IdleAfterSpawn;

    [Header("Identidad única del enemigo")]
    [SerializeField] private float enemySpreadRange = 1.5f; // qué tan disperso puede caer cada enemigo
    private float enemyUniqueOffset;

    //var para la funcion de detectar camara &&  IdleAfterSpawn case
    private bool isOnCamRange = false;

    //lista de Changeable Vars je nach case context
    //TimetoIdle
    private float speed;//testear si puedo usar este speed para todo ekisdfeeee
    private float currentTime;
    private float MaxTime = 3f;
    //Approach
    private float currentTimeApproach;
    private float MaxTimeApproach = 1.5f;
    //Condicion state = ExtremeSway
    public bool FiveEnemiesOnRange = false;
    private int swayStep = 0;
    private float swayTimer = 0f;
    //Condicion state = WaitPhase 3 onrANGE
    public bool ThreeEnemiesOnRange = false;
    // case logic WaitPhase
    private int vueltaCount = 0;
    private float previousSide = 0f;
    //camping config
    private Vector2[] campingOffsetsIzquierda = new Vector2[] {
    new Vector2(2.37f, 1.90f),
    new Vector2(2f, 1.20f) };
    private Vector2[] campingOffsetsDerecha = new Vector2[] {
    new Vector2(2.37f, 1.90f),
    new Vector2(2f, 1.20f) };
    // CrossRange
    public float hoverSpeed = 9f;   // velocidad del vaivén
    private float hoverOffset = 0f;
    private int hoverDirection = 1; // 1 = derecha, -1 = izquierda // no puede ser más q 1 (se queda en 1)
    // Attack
    public float damagePercent = 10f;

    //me esta falttando porner todos los anims y testing

    private int waypointIndex = 0;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        enemyTarget = GameObject.FindGameObjectWithTag("Enemy");
       // anim = GetComponent<Animator>();
        scriptHealth = GetComponent<Health>();
        scriptPMovement = playerTarget.GetComponent<pMovement>();

        if (scriptPMovement == null)
            Debug.LogError(gameObject.name + ": el GameObject 'Player' no tiene el script pMovement.");

        enemyUniqueOffset = Random.Range(-enemySpreadRange, enemySpreadRange);
    }

    void Update()
    {
        //main updated position
        Vector3 PlayerTargetPos = playerTarget.transform.position;
        Vector3 EnemyTargetPos = enemyTarget.transform.position;
        Vector3 myPos = transform.position;

       isOnCamRange = scriptCamera.IsInsideCameraBounds();

        print(isOnCamRange);

        Vector2 dirHaciaEnemigo = (myPos - PlayerTargetPos).normalized; //para lógica de detección (saber dónde está el enemigo respecto al jugador).
        Vector3 direction = (PlayerTargetPos - myPos).normalized; //para lógica de persecución (hacer que el enemigo avance hacia el jugador).
        float relacionMirada = Vector2.Dot(scriptPMovement.FacingDirection, dirHaciaEnemigo);
        /*Vector2 nos dice si dos vectores apuntan en la misma dirección, en direcciones opuestas
         * o si son perpendiculares.*/

        CheckingVidavar();

        switch (state)
        {

            case DummyState.IdleAfterSpawn:
                { 
                transform.position += direction * Maxspeed * Time.deltaTime;

                if (!isOnCamRange)
                {
                    transform.position += direction * Maxspeed * Time.deltaTime;
                }
                else
                {
                    currentTime += Time.deltaTime;

                    if (currentTime >= MaxTime)
                    {
                        currentTime = 0f;
                        state = DummyState.Approach; // transición directa, sin depender de "==0"
                    }
                }

                if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }
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

                    currentTimeApproach += Time.deltaTime;
                    if (currentTimeApproach >= MaxTimeApproach)
                    {
                        if (EnemyTargetPos.y != 1.39366)
                        //deberia ser y. 
                        { state = DummyState.WalkBehind; }
                        else transform.position += direction * Maxspeed * Time.deltaTime;
                    }

                    if (FiveEnemiesOnRange)
                    { state = DummyState.ExtremeSway; }
                    else transform.position += direction * Maxspeed * Time.deltaTime;

                    if (ThreeEnemiesOnRange)
                    { state = DummyState.WaitPhase; }
                    else transform.position += direction * Maxspeed * Time.deltaTime;

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;

            case DummyState.WalkBehind:
                {
                    Vector3 NewtargetPos = new Vector3(PlayerTargetPos.x + enemyUniqueOffset, 1.39366f, transform.position.z);

                    transform.position = Vector3.MoveTowards(transform.position, NewtargetPos, speed * Time.deltaTime);

                    if (transform.position.x == PlayerTargetPos.x)
                    { state = DummyState.Approach; }
                    //moverse desde su posicion hacia x.1.39366, hasta el playerTarget x luego cambiar

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;
            case DummyState.WaitPhase:
                {
                    float distanceBeforeMove = Vector3.Distance(PlayerTargetPos, myPos);

                    if (distanceBeforeMove <= radiusMovement)
                    {
                        side = (myPos.x >= PlayerTargetPos.x) ? -1f : 1f;

                        // si el lado cambió respecto al anterior, se completó una vuelta
                        if (side != previousSide && previousSide != 0f)
                        {
                            vueltaCount++;
                        }
                        previousSide = side;

                        //identifico myPos.x si esta a la izquierda o a la derecha
                        //si esta en izquierda/ numerosNegativos -> 
                        //myPos.x esta en derecha/numerosPositivos se suman
                        //-> se desplaza más / 2.0 = x
                    }

                    Vector3 newTargetPos = (side == -1f)
                        ? new Vector3(myPos.x - 2f, transform.position.y, transform.position.z)
                        : new Vector3(myPos.x + 2f, transform.position.y, transform.position.z);

                    transform.position = Vector3.MoveTowards(transform.position, newTargetPos, Maxspeed * Time.deltaTime);

                    // ¿ya cumplió las vueltas necesarias y esta vez se acercó en vez de alejarse?
                    if (vueltaCount >= vueltasParaAtacar)
                    {
                        float distanceAfterMove = Vector3.Distance(PlayerTargetPos, transform.position);

                        if (distanceAfterMove < distanceBeforeMove)
                        {
                            state = DummyState.Attacking;
                            vueltaCount = 0; // reset para la próxima vez que vuelva a WaitPhase
                        }
                    }
                    //recomendacion de reseteo extra en otra funcion pero yolo 
                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;
            case DummyState.Camping:
                {
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

                    transform.position = Vector3.MoveTowards(transform.position, newTargetPos, Maxspeed * Time.deltaTime);

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
                            state = DummyState.Attacking;
                            vueltaCount = 0;
                            waypointIndex = 0;
                        }
                    }

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }
                }
                break;
            case DummyState.CrossRange:
                {
                    if (Vector3.Distance(PlayerTargetPos, myPos) <= radiusMovement)
                    {
                        side = (myPos.x >= PlayerTargetPos.x) ? -1f : 1f;
                        //al llegar al rango minimo de alcance, calculala posición
                        Vector3 sidePos = PlayerTargetPos + new Vector3(sideOffset * side + enemyUniqueOffset, 0f, 0f);
                        transform.position += direction * Maxspeed * Time.deltaTime;

                        if (Vector3.Distance(myPos, sidePos) > 0.05f)
                        {
                            transform.position += direction * Maxspeed * Time.deltaTime;
                        }
                        else
                        {
                            hoverOffset += hoverDirection * hoverSpeed * Time.deltaTime;
                        }
                    
                    if (Vector3.Distance(PlayerTargetPos, myPos) < radiusMovement)
                       { state = DummyState.WaitPhase; }
                    else if (Vector3.Distance(PlayerTargetPos, myPos) > radiusMovement)
                       { state = DummyState.WaitPhase; }
                    }

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;
            case DummyState.ExtremeSway:
                {
                    float dirSide = (side == 0f) ? 1f : side;

                    //creando nuestra cinematica
                    switch (swayStep)
                    {
                        case 0: // a. camina rápido hacia el player
                            //expected fast anim will be replaced 4 a big jump thing SetAnim("Run");
                            swayStep = 1;
                            break;

                        case 1:
                            if (Vector3.Distance(transform.position, playerTarget.transform.position) > radiusMovement)
                            {
                                Vector3 dir = (playerTarget.transform.position - transform.position).normalized;
                                transform.position += dir * (speed * 2.5f) * Time.deltaTime;
                            }
                            else
                            {
                                swayStep = 2;
                            }
                            break;

                        case 2: // b. retrocede a la esquina, más lento
                            Vector3 corner = transform.position + new Vector3(sideOffset * dirSide, 0f, 0f);
                            if (Vector3.Distance(transform.position, corner) > 0.1f)
                            {
                                Vector3 dir = (corner - transform.position).normalized;
                                transform.position += dir * (speed * 1.5f) * Time.deltaTime;
                            }
                            else
                            {
                                swayStep = 3;
                            }
                            break;

                        case 3: // c. salto directo al rango del player
                            //expected fast anim will be replaced 4 a big jump thing SetAnim("Run");
                            swayTimer = 0.5f; // TODO: igualar a la duracion real del clip de salto
                            swayStep = 4;
                            break;

                        case 4:
                            swayTimer -= Time.deltaTime;
                            if (swayTimer <= 0f)
                            {
                                transform.position = PlayerTargetPos + new Vector3(radiusMovement * dirSide + enemyUniqueOffset, 0f, 0f);
                                swayStep = 5;
                            }
                            break;

                        case 5: // d. ejecuta ataque
                            state = DummyState.Attacking;
                            swayStep = 0; // reset para la próxima vez que entre a ExtremeSway
                            break;
                    }

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;
            case DummyState.Recovery:
                {
                    if (!scriptHealth.isRecovering)
                        {
                            //SetAnim("Recover");
                            scriptHealth.StartRegeneration(); 
                        }

                    scriptHealth.TickRegeneration();
                    if (!scriptHealth.isRecovering)
                        {
                             scriptHealth.isRecovering = false;
                        } 
                    }

                if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                break;
            case DummyState.Attacking:
                 {
                    //SetAnim("Attack");
                    playerTarget.GetComponent<Health>().Vida -= damagePercent;

                    Vector3 filteredPlayerPosX = new Vector3(
                    playerTarget.transform.position.x > 3.5f ? playerTarget.transform.position.x : transform.position.x,
                    transform.position.y,
                    transform.position.z
                    );

                    Vector3 filteredPlayerPosY = new Vector3(
                    transform.position.x,
                    playerTarget.transform.position.x > 3.5f ? playerTarget.transform.position.y : transform.position.y,
                    transform.position.z
                    );

                    if (Vector3.Distance(filteredPlayerPosX, myPos) > radiusMovement)
                    //moviendose en x
                    { state = DummyState.WaitPhase; }
                    else if (Vector3.Distance(filteredPlayerPosY, myPos) > radiusMovement)
                    //moviendose en y
                    { state = DummyState.Camping; }

                    if (scriptHealth.Vida <= 0) { state = DummyState.Dead; }

                }
                break;
             case DummyState.Dead:
                 {
                    //SetAnim("Death");
                    Destroy(gameObject/*, deathAnimDuration*/);

                 }
                  break;
             default:
                  break;

                    }
                }

    private void CheckingVidavar()
    {
        if (scriptHealth.Vida <= scriptHealth.recoveryThreshold) { state = DummyState.Recovery; }
        if (scriptHealth.Vida <= 0) { state = DummyState.Dead; } 

    }



}
