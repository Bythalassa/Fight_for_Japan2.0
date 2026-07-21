using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public CanvasLife canvasLife;
    [Header("Vida")]
    public int maxVida;   // 100% de vida
    public int vida;      // vida actual

    [Header("Estado")]
    public bool isDead = false;
    public bool onPelea = false;
    public bool destroyOnDeath = true;

    [Header("Recuperacion (solo la usan los enemies, pero vive aca porque el script se comparte)")]
    public float recoveryThreshold; // umbral (de maxVida) que dispara OnRecoveryThreshold
    public float recoveryDuration = 5f;
    public bool recoveryThresholdReached = false;
    public bool isRecovering = false;
    //Threshold = Límite de tolerancia

    private float curacionPorSegundo;
    private float recoveryElapsed;
    private float vidaAcumulada;

    private void Start()
    {
        vidaAcumulada = vida; 
    }
    public void TakeDamage(float damagePercent)
    {
        if (isDead)
        {
            return;
        }

        float actualDamage = maxVida * (damagePercent / 100f);
        vidaAcumulada -= actualDamage;
        vida = (int)vidaAcumulada; //se convierte en int al mostrar
        onPelea = true;

        if (canvasLife != null)
        {
            canvasLife.ActualizarBarra(vida, maxVida);
        }

        Debug.Log(gameObject.name + " recibio " + damagePercent + "% (" + actualDamage + " pts). Vida restante: " + vida);

        if (vida <= 0)
        {
            vida = 0;
            isDead = true;

            if (canvasLife != null)
            {
                SceneManager.LoadScene("Defeated");
            }
            else if (destroyOnDeath)
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

        public void Heal(int amount)
        {
        if (isDead) { return; }

            vidaAcumulada += amount;
            if (vidaAcumulada > maxVida)
            {
            vidaAcumulada = maxVida;
            }

        vida = (int)vidaAcumulada;

        /*int vidaEntera = (int)(vida * 100f);
        vida = vidaEntera / 100; redondeo decimales inecesario*/

        if (canvasLife != null)
        {
            canvasLife.ActualizarBarra(vida, maxVida);
        }
        }

    public void StartRegeneration()
    {
        Debug.Log("entered to function StartRegeneration");
        isRecovering = true;
        float vidaFaltante = maxVida - vida;
        curacionPorSegundo = vidaFaltante / recoveryDuration;
        recoveryElapsed = 0f;
        vidaAcumulada = vida;
        Debug.Log("RegenerateOverTime started");
    }

    public void TickRegeneration()
    {
        Debug.Log("entered to function TickRegeneration");
        if (!isRecovering) return;

        recoveryElapsed += Time.deltaTime;
        vidaAcumulada += curacionPorSegundo * Time.deltaTime;
        vida = (int)vidaAcumulada;

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

