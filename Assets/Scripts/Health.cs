using UnityEngine;
using System.Collections; //component 4m: IEnumerator

public class Health : MonoBehaviour
{
    [Header("Vida")]
    public float maxVida = 10f;   // 100% de vida
    public float Vida = 10f;      // vida actual

    [Header("Estado")]
    public bool isDead = false;
    public bool isInvulnerable = false;
    public bool OnPelea = false;

    [Header("Recuperacion (solo la usan los enemies, pero vive aca porque el script se comparte)")]
    public float recoveryThreshold; // umbral (de maxVida) que dispara OnRecoveryThreshold
    public float recoveryDuration = 5f;
    public bool recoveryThresholdReached = false;
    public bool isRecovering = false;
    //Threshold = Límite de tolerancia
   

    public void TakeDamage(float damagePercent)
    {
        if (isInvulnerable || isDead)
        {
            Debug.Log(gameObject.name + " esta invulnerable/muerto, no recibe daño.");
            return;
        }

        float actualDamage = maxVida * (damagePercent / 100f);
        Vida -= actualDamage;
        OnPelea = true;

        Debug.Log(gameObject.name + " recibio " + damagePercent + "% (" + actualDamage + " pts). Vida restante: " + Vida);

        if (Vida <= 0f)
        {
            Vida = 0f;
            isDead = true;
            return;
        }

        if (!recoveryThresholdReached && Vida <= recoveryThreshold)
        {
            recoveryThresholdReached = true;
        }
    }

    //estado de recuperación
    public IEnumerator RegenerateOverTime()
    {
        isRecovering = true;
        float vidaFaltante = maxVida - Vida;
        float curacionPorSegundo = vidaFaltante / recoveryDuration;

        float elapsed = 0f;
        while (elapsed < recoveryDuration)
        {
            elapsed += Time.deltaTime;
            Vida += curacionPorSegundo * Time.deltaTime;

            if (Vida > maxVida) Vida = maxVida; // por si se pasa un poco
            yield return null;
        }

        Vida = maxVida;
        recoveryThresholdReached = false; // puede volver a dispararse mas adelante
        isRecovering = false;
    }

    public void ResetRecoveryFlag()
    {
        recoveryThresholdReached = false;
    }
}