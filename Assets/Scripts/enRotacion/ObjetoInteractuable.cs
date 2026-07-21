using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{

    public float timeToRespawn = 3.0f;

    private float timeRespawn;
    private bool interactuable = true;
    private SpriteRenderer sprite;
    private Health playerHealth;
    public Health myHealth;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

    }

    void Update()
    {

        playerHealth = Relevo.CurrentPlayer.GetComponent<Health>();


        if (interactuable && myHealth.vida <=0 )
        {
            
            if (playerHealth != null)
            {
                playerHealth.vida += 100;
            }
            sprite.enabled = false;
            interactuable = false;
            timeRespawn = 0f;
        }

        if (!interactuable)
        {
            timeRespawn += Time.deltaTime;

            if (timeRespawn >= timeToRespawn)
            {
                interactuable = true;
                sprite.enabled = true;
            }
            return;
        }

    }
}
