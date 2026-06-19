using UnityEngine;

public class EnemySombra : MonoBehaviour
{
    public float vida = 100f;

    public float MaxTime = 6;
    public float currentTime;


    public void RecibirDamage(float damage)
    {
        vida -= damage;

        if (vida <= 0f)
        {
            Destroy(gameObject);
        }
    }


    // spawner que tiene que ver con hacer el sprite invisible
    // y luego visible con un cooldown de 6 secs

    public void TimerToDoSmt()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= MaxTime)
        {
            //-> sprite be invisible
            

            currentTime = 0;
        }
    }


}