using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LobbyPlayerAttack : MonoBehaviour
{
    public List<Health> targets = new List<Health>();
    public List<Image> targetPU = new List<Image>();
    public List<Image> nottargetPU = new List<Image>();

    public float damagePercent = 10f; // ahora es un %, no un numero fijo (10 = 10%)
    private bool isAbleToAttack;
    public float radiusAttack = 3.5f;

    public bool IsAttacking { get; private set; } // usando esto en DummyMovement
    public float attackSignalDuration = 0.2f; // cuanto tiempo se mantiene IsAttacking en true tras golpear
    private float attackSignalTimer = 0f;

    //PowerUp:D
    public bool AtacarPowerUp;
    private float dañoPU = 1f;
    private float dañoPUT;
    private float powerUpDuration = 2.6f;   // cuánto dura activo el power-up
    private float cooldownDuration = 2.5f;
    private float timer;

    public void Start()
    {
        timer = powerUpDuration;

        GameObject[] notobjetosPU = GameObject.FindGameObjectsWithTag("notPU");
        foreach (GameObject obj in notobjetosPU)
        {
            Image sr = obj.GetComponent<Image>();

            Debug.Log(obj + "in notObjetosPU");

            if (sr != null)
            {
                nottargetPU.Add(sr);
                sr.enabled = true;
                Debug.Log(obj.name + " enabled = " + sr.enabled);
            }
        }

        GameObject[] objetosPU = GameObject.FindGameObjectsWithTag("RellenoPU");
        foreach (GameObject obj in objetosPU)
        {
            Image sr = obj.GetComponent<Image>();
            Debug.Log(obj + "in objetosPU");
            if (sr != null)
            {
                targetPU.Add(sr);
                sr.enabled = false;
                Debug.Log(obj.name + " enabled = " + sr.enabled);

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
        PowerUpTimer();

        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            isAbleToAttack = true;
            IsAttacking = true;

            foreach (Health target in targets)
            {
                Debug.Log("hay un target en zona");
                if (target != null)
                {
                    target.TakeDamage(2); //debvug esta linea, testear valor de daño 

                    // Debug.Log("Atacando con " + damagePercent + "%" );
                    Debug.Log("Daño normal (fijo): " + 2);
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

        if (Keyboard.current.cKey.wasPressedThisFrame && AtacarPowerUp) // debug esta linea 
        {
            foreach (Health target in new List<Health>(targets))
            {
                if (target != null)
                {
                    target.TakeDamage(dañoPUT); //debvug esta linea  //debvug esta linea, testear valor de daño 
                    Debug.Log(dañoPUT + "dañoPUT value");
                }
            }

            foreach (Image sr in targetPU) { sr.enabled = false; }   
            foreach (Image sr in nottargetPU) { sr.enabled = true; }
            
            AtacarPowerUp = false;
            timer = powerUpDuration;
        }
    }

    public void PowerUpTimer()
    {

        timer -= Time.deltaTime;
        if (timer < -cooldownDuration)
        {
            AtacarPowerUp = true;
            dañoPUT = dañoPU *= 4f;

            foreach (Image sr in targetPU) { sr.enabled = true; }
            foreach (Image sr in nottargetPU) { sr.enabled = false; }
            timer = powerUpDuration;
        }
    }
}





