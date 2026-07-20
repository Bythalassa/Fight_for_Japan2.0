using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;



public class Health : MonoBehaviour
{
    public CanvasLife canvasLife;
    [Header("Vida")]
    public float maxVida;   // 100% de vida
    public float vida;      // vida actual

    [Header("Estado")]
    public bool isDead = false;
    public bool isInvulnerable = false;
    public bool onPelea = false;

    [Header("Recuperacion (solo la usan los enemies, pero vive aca porque el script se comparte)")]
    public float recoveryThreshold; // umbral (de maxVida) que dispara OnRecoveryThreshold
    public float recoveryDuration = 5f;
    public bool recoveryThresholdReached = false;
    public bool isRecovering = false;
    //Threshold = Límite de tolerancia

    private float curacionPorSegundo;
    private float recoveryElapsed;

    public void TakeDamage(float damagePercent)
    {
        Debug.Log("entered to function TakeDamage"); 
        if (isInvulnerable || isDead)
        {
            Debug.Log(gameObject.name + " esta invulnerable/muerto, no recibe daño.");
            return;
        }

        float actualDamage = maxVida * (damagePercent / 100f);
        vida -= actualDamage;
        onPelea = true;

        if (canvasLife != null)
        {
            canvasLife.ActualizarBarra(vida, maxVida);
        }

        Debug.Log(gameObject.name + " recibio " + damagePercent + "% (" + actualDamage + " pts). Vida restante: " + vida);

        if (vida <= 0f)
        {
            vida = 0f;
            isDead = true;

            if (canvasLife != null)
            {
                SceneManager.LoadScene("Defeated");
            }
            else
            {
                Destroy(gameObject);
            }





            return;
        }

        if (!recoveryThresholdReached && vida >= recoveryThreshold) // bruder: >= cuando a llegado al número o es menor,
                                                                    // si no es igual entonces no puede ser mayor, pero si puede ser menor 
        {
            recoveryThresholdReached = true;
        }
    }

    public void StartRegeneration()
    {
        Debug.Log("entered to function StartRegeneration");
        isRecovering = true;
        float vidaFaltante = maxVida - vida;
        curacionPorSegundo = vidaFaltante / recoveryDuration;
        recoveryElapsed = 0f;
        Debug.Log("RegenerateOverTime started");
    }

    public void TickRegeneration()
    {
        Debug.Log("entered to function TickRegeneration");
        if (!isRecovering) return;

        recoveryElapsed += Time.deltaTime;
        vida += curacionPorSegundo * Time.deltaTime;

        if (vida > maxVida) vida = maxVida;

    Debug.Log(gameObject.name + " Health is equal to " + vida);

    if (recoveryElapsed >= recoveryDuration)
    {
        vida = maxVida;
        recoveryThresholdReached = false; // puede volver a dispararse mas adelante
        isRecovering = false;
    }
    }

}