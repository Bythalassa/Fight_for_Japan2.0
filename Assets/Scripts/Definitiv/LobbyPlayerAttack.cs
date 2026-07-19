using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyPlayerAttack : MonoBehaviour
{
    public List<Health> targets = new List<Health>();
    public List<SpriteRenderer> targetPU = new List<SpriteRenderer>();

    public float damagePercent = 10f; // ahora es un %, no un numero fijo (10 = 10%)
    private bool isAbleToAttack;
    public float radiusAttack = 3.5f;

    public bool IsAttacking { get; private set; } // usando esto en DummyMovement
    public float attackSignalDuration = 0.2f; // cuanto tiempo se mantiene IsAttacking en true tras golpear
    private float attackSignalTimer = 0f;

    //PowerUp:D
    private float dañoPU = 1f;
    private float dañoPUT;
    public float powerUpDuration = 3f;   // cuánto dura activo el power-up
    private float powerUpTimer;
    private bool isPowerUpActive = false;

    public void Start()
    {
        GameObject[] objetosPU = GameObject.FindGameObjectsWithTag("RellenoPU");
        foreach (GameObject obj in objetosPU)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                targetPU.Add(sr);
                sr.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<Health>(out Health Vida))
            {
                targets.Add(Vida);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        { 
            if (collision.TryGetComponent<Health>(out Health Vida))
            {
                targets.Remove(Vida);
            }
        }
    }

    private void Update()
    {
        UpdatePowerUp();

        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            isAbleToAttack = true;
            IsAttacking = true;

            foreach (Health target in new List<Health>(targets))
            {
                Debug.Log("hay un target en zona");
                if (target != null)
                {
                    //no llega a esta linea 
                    target.TakeDamage(2);
                    if (isPowerUpActive)
                    {
                        target.TakeDamage(dañoPUT);
                    }

                    Debug.Log("Atacando a " + targets.Count + " enemigos con " + damagePercent + "%");
                }
            }
            isAbleToAttack = false;

            // arranca (o reinicia) la ventana en la que IsAttacking se reporta como true
            attackSignalTimer = attackSignalDuration;


            if (attackSignalTimer > 0f)
            {
                IsAttacking = true;
                attackSignalTimer -= Time.deltaTime;
            }
            else
            {
                IsAttacking = false;
            }

        }        

    }

    /*
    comportamiento esperado: 
    //no tiene nada que ver si esta peleando o no 
    ++es un temporizador : que siempre esta corriendo de 8 segundos para abajo 
    ++si esta en 0> sprite enabled  y el daño es =* 2 
    ++ si bool AtacarPowerUp (se activa al llegar a 0 el temporizador y automaticamente el daño es multiplicado)
    ++ lo apaga cuando presiona X YY AtacarPowerUp esta prendido = true { entonces AtacarPowerUp = false , timer = 8f 
    ++ de forma q se resete el timer} */



    public void ActivatePowerUp()
    {
        isPowerUpActive = true;
        powerUpTimer = powerUpDuration;
        if (powerUpTimer > 0f)
        {
            dañoPUT = dañoPU *= 2f;
            foreach (SpriteRenderer sr in targetPU)
            {
                sr.enabled = true;
            }
            powerUpTimer -= Time.deltaTime;

        if (powerUpTimer <= 0f) { isPowerUpActive = false; }
        }
    }

    private void UpdatePowerUp()
    {
        if (!isPowerUpActive) return;

        powerUpTimer -= Time.deltaTime;
        if (powerUpTimer <= 0f)
        {
            isPowerUpActive = false;
            foreach (SpriteRenderer sr in targetPU)
            {
                sr.enabled = false;
            }
        }


    }

}

    /*void VerificarEnemigosEnRangoDeAtaque()
    {
        Vector3 myPos = transform.position;
        foreach (var enemy in passiveRadiusScript.targets)
        {
            if (enemy == null) continue;

            Vector3 targetPos = enemy.transform.position;
            float distancia = Vector3.Distance(targetPos, myPos);

            if (distancia < radiusAttack)
            {
                if (!EnemigosenAttackRange.Contains(enemy))
                {
                    EnemigosenAttackRange.Add(enemy);
                }
            }
            else
            {
                if (EnemigosenAttackRange.Contains(enemy))
                {
                    EnemigosenAttackRange.Remove(enemy);
                }
            }
        }

        //optimización: Limpiar la lista de enemigos destruidos o que salieron por completo del radar pasivo
        EnemigosenAttackRange.RemoveAll(enemy => enemy == null || !passiveRadiusScript.targets.Contains(enemy));

        bool hayTresEnRango = EnemigosenAttackRange.Count == 3;

        foreach (var enemy in EnemigosenAttackRange)
        {
            enemy.ThreeEnemiesOnRange = hayTresEnRango;
        }*/





