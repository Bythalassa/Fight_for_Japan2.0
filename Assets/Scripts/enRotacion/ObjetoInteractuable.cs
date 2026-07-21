using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    public enum Estado { Disponible, Consumido, Esperando }

    public float timeToRespawn = 3.0f;
    public int vidaParaJugador = 5;
    private float timeRespawn;
    private Estado estadoActual = Estado.Disponible;
    private SpriteRenderer sprite;
    public Health myHealth;


    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (myHealth != null)
        {
            myHealth.destroyOnDeath = false; // que no se autodestruya
        }
    }

    void Update()
    {
        Health playerHealth = Relevo.CurrentPlayer.GetComponent<Health>();

        switch (estadoActual)
        {
            case Estado.Disponible:
                if (myHealth.isDead)
                {
                    DarVidaAlJugador();
                    sprite.enabled = false;
                    timeRespawn = 0f;
                    estadoActual = Estado.Esperando;
                }
                break;

            case Estado.Esperando:
                timeRespawn += Time.deltaTime;
                if (timeRespawn >= timeToRespawn)
                {
                    sprite.enabled = true;
                    myHealth.vida = myHealth.maxVida;
                    myHealth.isDead = false;
                    estadoActual = Estado.Disponible;
                }
                break;
        }


    }

    void DarVidaAlJugador()
    {
        Health playerHealth = Relevo.CurrentPlayer.GetComponent<Health>();
        if (playerHealth == null) return;

        playerHealth.Heal(vidaParaJugador);
    }
}