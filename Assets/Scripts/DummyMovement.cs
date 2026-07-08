using UnityEngine;
using System.Collections;

//might need this script later
public enum DummyState
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
    public DummyState state = DummyState.Spawn;

    [Header("Referencias")]
    private GameObject player;
    private Animator anim;
    private HeallthManager health; // vida de ESTE enemigo

    [Header("Movimiento general")]
    public float speed = 3f;
    public float radiusMovement = 3f;
    public float sideOffset = 20f;
    private float side; // -1 izquierda / 1 derecha respecto al player

    [Header("Vaiven (hover) - Camping")]
    public float hoverRange = 5.5f;
    public float hoverSpeed = 9f;
    private float hoverOffset = 0f;
    private int hoverDirection = 1;

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
    public float recoveryLifeThreshold = 8f; // vida <= a esto activa recovery (de un maximo de 10)
    public float recoveryDuration = 2f;
    private bool inRecovery = false;

    [Header("Pasos (WaitPhase / ExtremeSway)")]
    public float stepDistance = 1f;

    private bool extremeSwayChecked = false; // solo puede pasar 1 vez en toda la partida
    private bool crossedRangeOnce = false;   // "Traspasar el rango" solo 1 vez

    //death handle
    private bool deathHandled = false;
    public float deathAnimDuration = 1.5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        health = GetComponent<HeallthManager>();

        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (player == null) return;

       // if (health != null && health.Vida <= 0 && state != DummyState.Dead)
        {
            StopAllCoroutines();
            state = DummyState.Dead;
        }

       // if (health != null && !inRecovery && state != DummyState.Dead && health.Vida <= recoveryLifeThreshold)
        {
            StartCoroutine(RecoveryRoutine());
        }


        switch (state)
        {
            case DummyState.Spawn:
            case DummyState.IdleAfterSpawn:
            case DummyState.WaitPhase:
            case DummyState.ExtremeSway:
            case DummyState.Recovery:
                // Estos estados se resuelven enteramente por Coroutine.
                break;

            case DummyState.WalkBehind:
                WalkBehindLogic();
                break;

            case DummyState.Approach:
                ApproachLogic();
                break;

            case DummyState.Camping:
                CampingLogic();
                break;

            case DummyState.CrossRange:
                CrossRangeLogic();
                break;

            case DummyState.Attacking:
                AttackingLogic();
                break;

            case DummyState.Dead:
                DeadLogic();
                break;
        }
    }

    //Spawn

    IEnumerator SpawnRoutine()
    {
        state = DummyState.Spawn;
        SetAnim("Idle");
        yield return new WaitForSeconds(idleSpawnTime); // 1b

        state = DummyState.IdleAfterSpawn;
        DecideNextState();
    }
    //IEnumerator is ...

    void DecideNextState() //cambios de estado je nach condiciones
    {
      //  if (health != null && health.Vida <= 0)
        {
            state = DummyState.Dead;
            return;
        }

        if (!extremeSwayChecked && CountEnemiesInScene() == enemiesForExtremeSway)
        {
            StartCoroutine(CheckExtremeSwayRoutine());
            return;
        }

        if (!IsAnyEnemyBehindPlayer())
        {
            state = DummyState.WalkBehind;
            return;
        }

        if (CountEnemiesNearPlayer() >= enemiesForWaitPhase) //var on top 
        {
            StartCoroutine(WaitPhaseRoutine());
        }
        else
        {
            state = DummyState.Approach;
        }
    }

    //WALK BEHIND (direccion fija por atras del player)

    void WalkBehindLogic()
    {
        Vector3 myPos = transform.position;
        Vector3 targetPos = player.transform.position;

        float dirX = (myPos.x <= targetPos.x) ? 1f : -1f;
        transform.position += new Vector3(dirX * speed * Time.deltaTime, 0f, 0f);
        SetAnim("Walk");

        // llego a una zona razonable para decidir que hacer
        if (Vector3.Distance(myPos, targetPos) <= radiusMovement * 2f)
        {
            DecideNextState();
        }
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
            state = DummyState.Camping;
            return;
        }

        // 2c: player ataca pero no se mueve -> traspasar rango
        if (IsPlayerAttacking() && !IsPlayerMoving())
        {
            state = DummyState.CrossRange;
            return;
        }

        Vector3 direction = (targetPos - myPos).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(targetPos, myPos) <= radiusMovement)
        {
            side = (myPos.x >= targetPos.x) ? -1f : 1f;
            state = DummyState.Attacking;
        }
    }

    //WAIT PHASE - toma de relevos (secuencia de pasos)

    //(esto necesita un re diseño urgente ) d momento dejamos q internet coopere
    // pero re diseñarlo 

    IEnumerator WaitPhaseRoutine()
    {
        state = DummyState.WaitPhase;
        SetAnim("Walk");

        float facing = FacingSign();

        yield return MoveSteps(Vector3.right * facing, 3); // a. cae 3 pasos adelante
        yield return MoveSteps(Vector3.left * facing, 3);  // b. retrocede 3 pasos
        yield return MoveSteps(Vector3.up, 3);              // c. 3 pasos hacia arriba, sin girar

        Vector3 dirToPlayer = (player.transform.position - transform.position).normalized;
        yield return MoveSteps(dirToPlayer, 4);             // d. avanza 4 pasos hacia el player

        SetAnim("Idle");
        yield return new WaitForSeconds(2f);                // e. idle 2s

        // f. si sigue rodeado, va a Campeo; si no, sigue acercandose normal
        if (CountEnemiesNearPlayer() >= enemiesForWaitPhase)
        {
            state = DummyState.Camping;
        }
        else
        {
            state = DummyState.Approach;
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

        hoverOffset += hoverDirection * hoverSpeed * Time.deltaTime;
        if (hoverOffset > hoverRange || hoverOffset < -hoverRange)
        {
            hoverDirection *= -1; // acercarse / alejarse
        }

        Vector3 diagonalMove = new Vector3(
            hoverDirection * speed * 0.5f,
            hoverDirection * speed * 0.25f,
            0f) * Time.deltaTime;

        transform.position += diagonalMove;

        if (CountEnemiesNearPlayer() < enemiesForWaitPhase && !IsPlayerMoving())
        {
            state = DummyState.Approach;
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
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            SetAnim("Idle");
            if (CountEnemiesNearPlayer() >= enemiesForWaitPhase)
            {
                state = DummyState.Camping;
            }
            // si no hay 3 enemigos, se queda quieto en idle (no hace nada mas)
        }
    }

    //VAIVEN EXTREMO (caso especial, solo con 5 enemigos en escena)
    IEnumerator CheckExtremeSwayRoutine()
    {
        extremeSwayChecked = true;
        state = DummyState.ExtremeSway;
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
        state = DummyState.Attacking;
        }

    //Recovery
    IEnumerator RecoveryRoutine()
    {
        inRecovery = true;
        state = DummyState.Recovery;
        SetAnim("Recover");

        if (health != null) health.isInvulnerable = true;

        yield return new WaitForSeconds(recoveryDuration);

        if (health != null) health.isInvulnerable = false;
        inRecovery = false;

        DecideNextState(); // vuelve a elegir el flujo normal
    }

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
            state = DummyState.Approach;
        }
    }

    void DoDamageToPlayer()
    {
        if (player.TryGetComponent<HeallthManager>(out HeallthManager playerHealth))
        {
            playerHealth.TakeDamage(damagePercent); // TODO: TakeDamage debe tratar esto como % de vida
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

    bool IsAnyEnemyBehindPlayer()
    {
        // TODO: definir "atras" segun hacia donde mira el player (facing real).
        // Por ahora asume que "atras" = mismo lado en X donde ya estan otros enemigos.
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies)
        {
            if (e == gameObject) continue;
            bool isBehind = (e.transform.position.x < player.transform.position.x);
            if (isBehind) return true;
        }
        return false;
    }

    bool IsPlayerMoving()
    {
        //return playerMovementScript.isMoving; REPLACE this to the players function
        return false;
    }

    bool IsPlayerAttacking()
    {
        //return compitaMovement.isMoving; REPLACE this to the players script and function
        return false;
    }





}