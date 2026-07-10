using UnityEngine;

public class HeallthManager : MonoBehaviour
{
    //planteando el daño porcentual

    public float maxHealth = 10f; // 100% de vida
    public float health = 10f;    // vida actual
    public bool isDead = false;
    public bool OnPelea = true;
    public bool isInvulnerable = false;

    public void TakeDamage(float damagePercent)
    {
        if (isInvulnerable)
        {
            Debug.Log(gameObject.name + " esta en recuperacion, no recibe daño.");
            return;
        }

        float actualDamage = maxHealth * (damagePercent / 100f);
        health -= actualDamage;
        OnPelea = true;
        Debug.Log(gameObject.name + " recibio " + damagePercent + "% (" + actualDamage + " pts). Vida restante: " + health);

        if (health <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }
}
