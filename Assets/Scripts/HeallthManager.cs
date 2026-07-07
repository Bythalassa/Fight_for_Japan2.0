using UnityEngine;

public class HeallthManager : MonoBehaviour
{
    public float health = 10;
    public bool isDead = false;
    public bool OnPelea = true;

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnPelea = true;
        Debug.Log(gameObject.name + " took " + damage + " damage. Remaining health: " + health);

        if (health <= 0)
        {
            isDead = true;
        }

        if (isDead)
        {
            Destroy(gameObject);

        }
    }
}