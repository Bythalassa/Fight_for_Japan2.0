using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public List<HeallthManager> targets = new List<HeallthManager>();

    public float damagePercent = 10f; // ahora es un %, no un numero fijo (10 = 10%)
    private bool isAbleToAttack;

    public bool IsAttacking { get; private set; } // usando esto en DummyMovement
    public float attackSignalDuration = 0.2f; // cuanto tiempo se mantiene IsAttacking en true tras golpear
    private float attackSignalTimer = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<HeallthManager>(out HeallthManager health))
            {
                targets.Add(health);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<HeallthManager>(out HeallthManager health))
            {
                targets.Remove(health);
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
            foreach (HeallthManager target in new List<HeallthManager>(targets))
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
    }
}
