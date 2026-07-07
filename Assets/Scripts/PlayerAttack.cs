using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public List<HeallthManager> targets = new List<HeallthManager>();

    public int damage = 10;
    private bool isAbleToAttack;

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
            Debug.Log("Atacando a " + targets.Count + " enemigos");

            foreach (HeallthManager target in new List<HeallthManager>(targets))
            {
                if (target != null) // por si ya fue destruido
                {
                    target.TakeDamage(damage);
                }
            }

            isAbleToAttack = false;
        }
    }
}
