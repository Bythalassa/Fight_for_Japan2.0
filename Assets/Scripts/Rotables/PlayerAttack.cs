using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public List<Health> targets = new List<Health>();
    public List<DummyMovementDospuntoZero> EnemigosenAttackRange = new List<DummyMovementDospuntoZero>();

    public float damagePercent = 10f; // ahora es un %, no un numero fijo (10 = 10%)
    private bool isAbleToAttack;
    public float radiusAttack = 3.5f;

    public bool IsAttacking { get; private set; } // usando esto en DummyMovement
    public float attackSignalDuration = 0.2f; // cuanto tiempo se mantiene IsAttacking en true tras golpear
    private float attackSignalTimer = 0f;

    //medir la distancia de todos los enemigos targeted desde la referencia del script PassiveRadius
    public PassiveRadius passiveRadiusScript;

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
        if (Input.GetKeyDown(KeyCode.X))
        {
            isAbleToAttack = true;
        }

        if (isAbleToAttack)
        {
            IsAttacking = true; 
            Debug.Log("Atacando a " + targets.Count + " enemigos con " + damagePercent + "%");
            
            foreach (Health target in new List<Health>(targets))
            {
                if (target != null)
                {
                    target.TakeDamage(damagePercent);
                }
            }
            isAbleToAttack = false;

            // arranca (o reinicia) la ventana en la que IsAttacking se reporta como true
            attackSignalTimer = attackSignalDuration;
        }

        if (attackSignalTimer > 0f)
        {
            IsAttacking = true;
            attackSignalTimer -= Time.deltaTime;
        }
        else
        {
            IsAttacking = false;
        }

        //VerificarEnemigosEnRangoDeAtaque();

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

        // optimización: Limpiar la lista de enemigos destruidos o que salieron por completo del radar pasivo
        EnemigosenAttackRange.RemoveAll(enemy => enemy == null || !passiveRadiusScript.targets.Contains(enemy));

        bool hayTresEnRango = EnemigosenAttackRange.Count == 3;

        foreach (var enemy in EnemigosenAttackRange)
        {
            enemy.ThreeEnemiesOnRange = hayTresEnRango;
        }*/
    }



    
