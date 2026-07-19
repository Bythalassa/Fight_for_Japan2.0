using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LobbyPlayerAttack : MonoBehaviour
{
    public List<Health> targets = new List<Health>();
    public List<SpriteRenderer> targetPU = new List<SpriteRenderer>();
    public List<SpriteRenderer> nottargetPU = new List<SpriteRenderer>();

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
    public float powerUpDuration = 8f;   // cuánto dura activo el power-up
    public float cooldownDuration = 5f;
    private float timer;
    private bool isPowerUpActive = false;

    public void Start()
    {
        timer = powerUpDuration;

        GameObject[] notobjetosPU = GameObject.FindGameObjectsWithTag("notPU");
        foreach (GameObject obj in notobjetosPU)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

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
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
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

            foreach (Health target in new List<Health>(targets))
            {
                Debug.Log("hay un target en zona");
                if (target != null)
                {
                    target.TakeDamage(2); //debvug esta linea 

                    Debug.Log("Atacando a " + targets.Count + " enemigos con " + damagePercent + "%");
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
                    target.TakeDamage(dañoPUT); //debvug esta linea 
                    AtacarPowerUp = false;
                    timer = powerUpDuration;
                    Debug.Log("Atacando  con PowerUp a " + targets.Count + " enemigos con " + damagePercent + "%");
                }

                GameObject[] notobjetosPU = GameObject.FindGameObjectsWithTag("notPU");
                foreach (GameObject obj in notobjetosPU)
                {
                    SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        nottargetPU.Add(sr);
                        sr.enabled = true;
                    }
                }
            }
        }

    }

    public void PowerUpTimer()
    {

        timer -= Time.deltaTime;
        if (timer < -cooldownDuration)
        {
            AtacarPowerUp = true;
            dañoPUT = dañoPU *= 2f;

            GameObject[] objetosPU = GameObject.FindGameObjectsWithTag("RellenoPU");
            foreach (GameObject obj in objetosPU)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    targetPU.Add(sr);
                    sr.enabled = true;
                }
            }

            //porqe estos no se apagan
            GameObject[] notobjetosPU = GameObject.FindGameObjectsWithTag("notPU");
            foreach (GameObject obj in notobjetosPU)
            {
                SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    nottargetPU.Add(sr);
                    sr.enabled = false;
                }
            }

            timer = powerUpDuration;

        }

    }
}





