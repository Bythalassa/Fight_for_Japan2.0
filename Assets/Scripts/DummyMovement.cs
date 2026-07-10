using UnityEngine;
using System.Collections;

//might need this script later
public enum DummyStateTwoo
{
    Spawn,
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

public class DummyMovement : MonoBehaviour
{
    [Header("Estado actual (solo lectura, para debug)")]
    public DummyStateTwoo state = DummyStateTwoo.Spawn;

    [Header("Referencias")]
    private GameObject player;
    private Animator anim;
    private Health health;  // vida de ESTE enemigo

    [Header("Tracking del player (independiente del script de movimiento del player)")]
    // En vez de depender de un componente especifico del player (pMovement u otro),
    // esto trackea la posicion del player CADA FRAME desde este mismo script y calcula
    // velocidad/facing/isMoving en base a esos deltas. Asi funciona sin importar que
    // script de movimiento tenga el player o donde este ese componente.
    public float playerMovingThreshold = 0.05f; // unidades/seg minimas para considerar "se esta moviendo"
    private Vector3 lastPlayerPos;
    private Vector3 playerVelocity;      // velocidad real del player, recalculada cada frame
    private bool playerIsMoving;
    private Vector2 lastKnownPlayerFacing = Vector2.down; // se actualiza solo cuando el player se mueve; si se detiene, mantiene el ultimo valor

    [Header("Movimiento general")]
    public float speed = 3f;
    public float radiusMovement = 3f;
    public float sideOffset = 20f;
    private float side; // -1 izquierda / 1 derecha respecto al player

    [Header("Vaiven (hover) - Camping")]
    public float hoverRange = 5.5f;
    public float hoverSpeed = 9f;

    [Header("Timers")]
    public float idleSpawnTime = 2f;      // 1b
    public float attackCooldown = 4f;     // 7
    private float attackTimer = 0f;

    [Header("Deteccion de grupo")]
    public int enemiesForWaitPhase = 3;     // 3 y 3.1
    public int enemiesForExtremeSway = 5;   // 5
    public float groupCheckRadius = 8f;     // que tan cerca del player cuenta como "cerca"

    [Header("Ataque / Daño (porcentual)")]
    public float damagePercent = 1f; // 1, 2 o 3 segun el enemigo (1%, 2%, 3%)

    [Header("Recuperacion")]
    //THE REST IS ON health 
    private bool inRecovery = false;

    [Header("Pasos (WaitPhase / ExtremeSway)")]
    public float stepDistance = 1f;

    private bool extremeSwayChecked = false; // solo puede pasar 1 vez en toda la partida
    private bool crossedRangeOnce = false;   // "Traspasar el rango" solo 1 vez

    //death handle
    private bool deathHandled = false;
    public float deathAnimDuration = 1.5f;

    [Header("Anti-overlap / distribucion entre enemigos")]
    public float minDistanceBetweenEnemies = 1.2f;   // debajo de esto, se empujan
    public float separationStrength = 1f;
    private static int enemySpawnCounter = 0;        // contador global para asignar slots
    private int mySlot;                              // slot unico de este enemigo

    [Header("Spawn - caminata inicial")]
    public float spawnWanderRadius = 4f;
    private Vector3 spawnTarget;

    [Header("Camping - waypoints (se calculan al entrar al estado, relativos a la pos actual)")]
    private Vector3 campWaypointA;
    private Vector3 campWaypointB;
    private bool campWaypointsSet = false;
    private bool movingToCampB = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();

        // Identifica al player por tag al momento de spawnear este enemigo,
        // y arranca el tracking de posicion desde ahi (sin esto, el primer
        // frame calcularia una velocidad falsa basada en Vector3.zero).
        if (player != null)
        {
            lastPlayerPos = player.transform.position;
        }

        // Slot unico para que cada enemigo tienda a un lado/orden distinto
        // y no todos apunten exactamente al mismo punto (causa del "se unen").
        mySlot = enemySpawnCounter++;
        side = (mySlot % 2 == 0) ? 1f : -1f;

        /* if (player != null) //porque me importa estas lineas no entiendo
         {
             playerAttack = player.GetComponent<PlayerAttackBase>(); // trae EspadachinAttack o CompitaAttack, lo que este en el player
         }*/

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (player == null) return;//wegen?
        if (health == null) return;//wegen?

        // Se actualiza SIEMPRE, en todos los frames, sin importar el estado actual.
        // Esto es lo que hace que el rango/facing sea relativo al movimiento REAL
        // del player en este instante, y no a una posicion vieja o cacheada.
        UpdatePlayerTracking();

        if (health.Vida <= 0 && state != DummyStateTwoo.Dead)
        {
            StopAllCoroutines();
            state = DummyStateTwoo.Dead;
            return;
        }

        if (!inRecovery && state != DummyStateTwoo.Dead && health.recoveryThresholdReached && !health.isRecovering)
        {
            Debug.Log("StartCoroutine(RecoveryRoutine()) started");
        }

        switch (state)
        {
            case DummyStateTwoo.Spawn:
            case DummyStateTwoo.IdleAfterSpawn:
            case DummyStateTwoo.WaitPhase:
            case DummyStateTwoo.ExtremeSway:
                // Estos estados se resuelven enteramente por Coroutine.
                break;

            case DummyStateTwoo.WalkBehind:
                WalkBehindLogic();
                break;

            case DummyStateTwoo.Approach:
                ApproachLogic();
                break;

            case DummyStateTwoo.Camping:
                CampingLogic();
                break;

            case DummyStateTwoo.CrossRange:
                CrossRangeLogic();
                break;

            case DummyStateTwoo.Attacking:
                AttackingLogic();
                break;

            case DummyStateTwoo.Dead:
                DeadLogic();
                break;
        }
    }

    //Spawn

    IEnumerator SpawnRoutine()
    {
        state = DummyStateTwoo.Spawn;
        SetAnim("Idle");
        yield return new WaitForSeconds(idleSpawnTime); // 1b

        // Punto random RELATIVO a donde el enemigo ya esta parado (no una posicion
        // fija del mundo calculada una sola vez al arrancar la escena).
        Vector2 randomDir2D = Random.insideUnitCircle.normalized;
        spawnTarget = transform.position + new Vector3(randomDir2D.x, randomDir2D.y, 0f) * spawnWanderRadius;

        SetAnim("Walk");
        while (Vector3.Distance(transform.position, spawnTarget) > 0.1f)
        {
            // Se recalcula la direccion CADA FRAME desde transform.position actual,
            // por eso el movimiento se ve continuo y no un salto/teleport.
            Vector3 dir = (spawnTarget - transform.position).normalized;
            Vector3 separation = GetSeparationOffset();
            transform.position += (dir * speed + separation * separationStrength) * Time.deltaTime;
            yield return null;
        }

        state = DummyStateTwoo.IdleAfterSpawn;
        DecideNextState();
    }
    void DecideNextState() //cambios de estado je nach condiciones
    {
        // El chequeo de muerte ya se maneja centralizado en Update(), no se duplica aqui.

        if (!extremeSwayChecked && CountEnemiesInScene() == enemiesForExtremeSway)
        {
            StartCoroutine(CheckExtremeSwayRoutine());
            return;
        }

        // Antes esto dependia de IsAnyEnemyBehindPlayer() (un placeholder) Y ademas
        // exigia el conteo de WaitPhase al mismo tiempo -> practicamente nunca se
        // cumplian las dos cosas juntas y WaitPhase no se ejecutaba nunca.
        // Ahora se chequean por separado:

        if (CountEnemiesNearPlayer() >= enemiesForWaitPhase)
        {
            StartCoroutine(WaitPhaseRoutine());
            return;
        }

        if (!IsAnyEnemyBehindPlayer())
        {
            state = DummyStateTwoo.WalkBehind;
            return;
        }

        state = DummyStateTwoo.Approach;
    }

    //WALK BEHIND (direccion fija por atras del player)

    void WalkBehindLogic()
    {
        Vector3 myPos = transform.position;
        Vector3 playerPos = player.transform.position;

        // ANTES: dirX se calculaba solo en X y el target era la posicion EXACTA
        // del player -> todos los enemigos convergian al mismo punto (por eso
        // se superponian) y el movimiento en Y no existia (incoherente).
        // AHORA: el target es un punto detras/al lado del player, usando el
        // "side" unico asignado a cada enemigo en Start(), mas una fuerza de
        // separacion para no pisarse entre ellos.
        Vector3 targetPos = playerPos + new Vector3(sideOffset * side, 0f, 0f);
        Vector3 separation = GetSeparationOffset();

        Vector3 toTarget = targetPos - myPos;
        if (toTarget.magnitude > 0.05f)
        {
            Vector3 direction = toTarget.normalized;
            transform.position += (direction * speed + separation * separationStrength) * Time.deltaTime;
            SetAnim("Walk");
        }

        // llego a una zona razonable para decidir que hacer
        if (Vector3.Distance(myPos, targetPos) <= radiusMovement)
        {
            DecideNextState();
        }
    }

    // Empuja al enemigo lejos de otros enemigos que esten demasiado cerca,
    // para que nunca terminen exactamente en la misma posicion.
    Vector3 GetSeparationOffset()
    {
        Vector3 push = Vector3.zero;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e == gameObject) continue;
            Vector3 diff = transform.position - e.transform.position;
            float dist = diff.magnitude;
            if (dist < minDistanceBetweenEnemies)
            {
                // si estan exactamente en el mismo punto (dist == 0), empuja en una
                // direccion arbitraria pero estable (basada en el slot) para desempatar
                Vector3 pushDir = dist > 0.0001f ? diff.normalized : new Vector3(Mathf.Cos(mySlot), Mathf.Sin(mySlot), 0f);
                push += pushDir * (minDistanceBetweenEnemies - dist);
            }
        }
        return push;
    }

    // APPROACH - acercamiento al rango
    void ApproachLogic()
    {
        SetAnim("Walk");
        Vector3 targetPos = player.transform.position;
        Vector3 myPos = transform.position;

        // 2b: player se mueve -> Campeo
        if (IsPlayerMoving())
        {
            state = DummyStateTwoo.Camping;
            return;
        }

        // 2c: player ataca pero no se mueve -> traspasar rango
        if (/*IsPlayerAttacking() &&*/ !IsPlayerMoving())
        {
            state = DummyStateTwoo.CrossRange;
            return;
        }

        Vector3 direction = (targetPos - myPos).normalized;
        Vector3 separation = GetSeparationOffset();
        transform.position += (direction * speed + separation * separationStrength) * Time.deltaTime;

        if (Vector3.Distance(targetPos, myPos) <= radiusMovement)
        {
            side = (myPos.x >= targetPos.x) ? -1f : 1f;
            state = DummyStateTwoo.Attacking;
        }
    }

    //WAIT PHASE - toma de relevos (secuencia de pasos)

    //(esto necesita un re diseño urgente ) d momento dejamos q internet coopere
    // pero re diseñarlo 

    IEnumerator WaitPhaseRoutine()
    {
        state = DummyStateTwoo.WaitPhase;
        SetAnim("Walk");

        float facing = FacingSign(); // 1 = player a la derecha, -1 = player a la izquierda

        // ANTES: right x3 / left x3 / up x3 -> no se parecia en nada al patron
        // zigzag pedido (paso lateral corto + retroceso diagonal cruzando el
        // punto de partida). AHORA se arma ese zigzag, relativo a la posicion
        // actual en cada tramo (start = transform.position dentro de
        // MoveToRelativeOffset), y se espeja segun hacia que lado esta el player.

        // Paso 1: paso lateral hacia el player, misma altura
        yield return MoveToRelativeOffset(new Vector3(facing * stepDistance * 2f, 0f, 0f));

        // Paso 2: retrocede en diagonal, cruzando el punto de partida y subiendo un poco
        yield return MoveToRelativeOffset(new Vector3(-facing * stepDistance * 3f, stepDistance, 0f));

        // Paso 3: avanza hacia el player (esto ya se calcula relativo, se mantiene igual)
        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        yield return MoveSteps(dirToPlayer, 4);             // avanza 4 pasos hacia el player

        SetAnim("Idle");
        yield return new WaitForSeconds(2f);                // idle 2s

        // si sigue rodeado, va a Campeo; si no, sigue acercandose normal
        if (CountEnemiesNearPlayer() >= enemiesForWaitPhase)
        {
            campWaypointsSet = false;
            state = DummyStateTwoo.Camping;
        }
        else
        {
            state = DummyStateTwoo.Approach;
        }
    }

    // Mueve suavemente desde la posicion ACTUAL hasta esa posicion + offset.
    // Se recalcula "start" cada vez que se llama, por eso el patron zigzag
    // siempre es relativo a donde el enemigo esta parado en ese momento.
    IEnumerator MoveToRelativeOffset(Vector3 offset)
    {
        Vector3 start = transform.position;
        Vector3 end = start + offset;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
    }

    IEnumerator MoveSteps(Vector3 direction, int steps)
    {
        Vector3 dir = direction.normalized;
        for (int i = 0; i < steps; i++)
        {
            Vector3 start = transform.position;
            Vector3 end = start + dir * stepDistance;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(start, end, t);
                yield return null;
            }
        }
    }

    float FacingSign()
    {
        return (transform.position.x <= player.transform.position.x) ? 1f : -1f;
    }

    //CAMPING - diagonal alrededor del rango del player

    void CampingLogic()
    {
        SetAnim("Walk");

        // ANTES: hoverOffset se acumulaba con un signo global y el desplazamiento
        // (diagonalMove) se sumaba directo a transform.position sin puntos de
        // referencia reales -> el "vaiven" no formaba un patron diagonal
        // reconocible, solo un drift que dependia de cuando se re-evaluaba.
        // AHORA: se calculan 2 waypoints diagonales EN EL MOMENTO en que se entra
        // a Camping, relativos a la posicion actual del enemigo, y se camina
        // entre ellos con MoveTowards (siempre usando transform.position actual).
        if (!campWaypointsSet)
        {
            Vector3 basePos = transform.position;
            float diagX = (mySlot % 2 == 0) ? 1f : -1f; // distinto por enemigo, evita que todos hagan el mismo dibujo
            Vector3 diagDir = new Vector3(diagX, 1f, 0f).normalized;

            campWaypointA = basePos;
            campWaypointB = basePos + diagDir * hoverRange;
            movingToCampB = true;
            campWaypointsSet = true;
        }

        Vector3 target = movingToCampB ? campWaypointB : campWaypointA;
        Vector3 separation = GetSeparationOffset();

        transform.position = Vector3.MoveTowards(transform.position, target, hoverSpeed * Time.deltaTime);
        transform.position += separation * separationStrength * Time.deltaTime;

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingToCampB = !movingToCampB;
            // recalcula el otro extremo relativo a la posicion actual, asi el
            // vaiven se sigue actualizando en vez de quedar fijo a un origen viejo
            if (movingToCampB)
                campWaypointB = transform.position + new Vector3((mySlot % 2 == 0) ? 1f : -1f, 1f, 0f).normalized * hoverRange;
            else
                campWaypointA = transform.position - new Vector3((mySlot % 2 == 0) ? 1f : -1f, 1f, 0f).normalized * hoverRange;
        }

        if (CountEnemiesNearPlayer() < enemiesForWaitPhase && !IsPlayerMoving())
        {
            campWaypointsSet = false; // se recalculan la proxima vez que entre a Camping
            state = DummyStateTwoo.Approach;
        }
    }

    // CROSS RANGE - traspasar el rango (una sola vez)
    void CrossRangeLogic()
    {
        Vector3 targetPos = player.transform.position;
        Vector3 myPos = transform.position;

        if (!crossedRangeOnce)
        {
            side *= -1f;
            crossedRangeOnce = true;
        }

        Vector3 sidePos = targetPos + new Vector3(sideOffset * side, 0f, 0f);

        if (Vector3.Distance(myPos, sidePos) > 0.05f)
        {
            SetAnim("Walk");
            Vector3 direction = (sidePos - myPos).normalized;
            Vector3 separation = GetSeparationOffset();
            transform.position += (direction * speed + separation * separationStrength) * Time.deltaTime;
        }
        else
        {
            SetAnim("Idle");
            if (CountEnemiesNearPlayer() >= enemiesForWaitPhase)
            {
                state = DummyStateTwoo.Camping;
            }
            // si no hay 3 enemigos, se queda quieto en idle (no hace nada mas)
        }
    }

    //VAIVEN EXTREMO (caso especial, solo con 5 enemigos en escena)
    IEnumerator CheckExtremeSwayRoutine()
    {
        extremeSwayChecked = true;
        state = DummyStateTwoo.ExtremeSway;
        SetAnim("Idle");

        yield return new WaitForSeconds(2f);

        // sigue muy lejos del rango del player -> TODO: ajustar el multiplicador de "muy lejos"
        if (Vector3.Distance(transform.position, player.transform.position) > radiusMovement * 3f)
        {
            yield return ExtremeSwaySequence();
        }
        else
        {
            DecideNextState(); // no ocurre, sigue el flujo normal
        }
    }

    IEnumerator ExtremeSwaySequence()
    {
        float fastSpeed = speed * 2.5f;
        float mediumSpeed = speed * 1.5f;

        // a. camina a mucha velocidad hacia el player
        SetAnim("Run");
        while (Vector3.Distance(transform.position, player.transform.position) > radiusMovement)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            transform.position += dir * fastSpeed * Time.deltaTime;
            yield return null;
        }

        // b. retrocede hasta la esquina, un poco mas lento
        Vector3 corner = transform.position + new Vector3(sideOffset * (side == 0 ? 1f : side), 0f, 0f);
        while (Vector3.Distance(transform.position, corner) > 0.1f)
        {
            Vector3 dir = (corner - transform.position).normalized;
            transform.position += dir * mediumSpeed * Time.deltaTime;
            yield return null;
        }

        // c. salto directo al rango del player
        SetAnim("Jump");
        yield return new WaitForSeconds(0.5f); // TODO: igualar a la duracion real del clip de salto
        transform.position = player.transform.position + new Vector3(radiusMovement * (side == 0 ? 1f : side), 0f, 0f);

        // d. ejecuta ataque
        state = DummyStateTwoo.Attacking;
    }

    //Recovery
   

    //ATTACKING
    void AttackingLogic()
    {
        attackTimer += Time.deltaTime;
        SetAnim("Idle");

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;
            SetAnim("Attack");
            DoDamageToPlayer();
        }

        // si el player se aleja del rango, vuelve a perseguir
        if (Vector3.Distance(transform.position, player.transform.position) > radiusMovement * 1.5f)
        {
            state = DummyStateTwoo.Approach;
        }
    }

    void DoDamageToPlayer()
    {
        if (player.TryGetComponent<Health>(out Health playerHealth))
        {
            //El debug de recibir daño esta en health xd 
            playerHealth.TakeDamage(damagePercent); // TakeDamage trata esto como % de vida
        }
    }

    //dead
    void DeadLogic()
    {
        if (deathHandled) return;
        deathHandled = true;

        SetAnim("Death");
        Destroy(gameObject, deathAnimDuration);
    }

    //Complementary
    void SetAnim(string trigger)
    {
        if (anim != null) anim.SetTrigger(trigger);
    }

    int CountEnemiesInScene()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    int CountEnemiesNearPlayer()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = 0;
        foreach (GameObject e in enemies)
        {
            if (Vector3.Distance(e.transform.position, player.transform.position) <= groupCheckRadius)
            {
                count++;
            }
        }
        return count;
    }

    // Recalcula velocidad/facing/isMoving del player EN VIVO, comparando su
    // posicion actual contra la del frame anterior. No depende de ningun
    // componente externo del player: solo necesita la referencia por tag.
    void UpdatePlayerTracking()
    {
        Vector3 currentPos = player.transform.position;
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        playerVelocity = (currentPos - lastPlayerPos) / dt;

        playerIsMoving = playerVelocity.sqrMagnitude > (playerMovingThreshold * playerMovingThreshold);

        if (playerIsMoving)
        {
            lastKnownPlayerFacing = new Vector2(playerVelocity.x, playerVelocity.y).normalized;
        }
        // si el player esta quieto, lastKnownPlayerFacing se mantiene con el ultimo valor real

        lastPlayerPos = currentPos;
    }

    bool IsAnyEnemyBehindPlayer()
    {
        // "Atras" ahora se define con el facing real del player (calculado en
        // UpdatePlayerTracking() a partir de su movimiento frame a frame, no de
        // un componente externo). Un enemigo esta "atras" si el vector desde
        // el player hacia ese enemigo apunta en contra del facing del player
        // (producto punto negativo).
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e == gameObject) continue;

            Vector2 toEnemy = e.transform.position - player.transform.position;
            if (toEnemy.sqrMagnitude < 0.0001f) continue; // mismo punto que el player, ignorar

            float dot = Vector2.Dot(lastKnownPlayerFacing.normalized, toEnemy.normalized);
            if (dot < 0f) return true; // del lado opuesto al facing -> "atras"
        }
        return false;
    }

    bool IsPlayerMoving()
    {
        return playerIsMoving;
    }

    /*bool IsPlayerAttacking()
    {
        Ya conectado a PlayerAttack.IsAttacking; si no hay referencia, asume que no ataca.
        //return playerAttack != null && playerAttack.IsAttacking;
    }*/
}