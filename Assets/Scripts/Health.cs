using UnityEngine;
using System.Collections;
using UnityEngine.AdaptivePerformance; //component 4m: IEnumerator

public class Health : MonoBehaviour
{
    [Header("Vida")]
    public float maxVida;   // 100% de vida
    public float Vida;      // vida actual

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

        if (!recoveryThresholdReached && Vida >= recoveryThreshold) // bruder: >= cuando a llegado al número o es menor, si no es igual entonces no puede ser mayor, pero si puede ser menor 
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
            Debug.Log("RegenerateOverTime started");
            elapsed += Time.deltaTime;
            Vida += curacionPorSegundo * Time.deltaTime;

            if (Vida > maxVida) Vida = maxVida; //esto solo tiene sentido si :  if (!recoveryThresholdReached && Vida == recoveryThreshold)
            yield return null;
            Debug.Log(gameObject.name + "Health is equal to" + Vida);
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