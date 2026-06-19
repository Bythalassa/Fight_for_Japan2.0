using UnityEngine;


public class EnemyNinjaRD : MonoBehaviour
{
    public float damage2;

    public GameObject TargetUno; //add Espadachin
    public GameObject TargetDos; //add Compita
    public float Speed2;

    public bool isAbleToAttack2 = true;
    public float radiusMovement2;
    public float raidusAttack2;

    public float cooldownAtaque2 = 1.5f;
    public float currentTime2;


    void Start()
    {

    }

    void Update()
    {
        if (!isAbleToAttack2)
        {
            currentTime2 += Time.deltaTime;
            if (currentTime2 >= cooldownAtaque2)
            {
                isAbleToAttack2 = true;
                currentTime2 = 0f;
            }
        }

        FollowTarget2();
    }

    public void FollowTarget2()
    {
        //Logica (que target esta más cerca)
        float distancia1 = Vector3.Distance(transform.position, TargetUno.transform.position); //ESPADACHIN 
        float distancia2 = Vector3.Distance(transform.position, TargetDos.transform.position); //COMPITA 

        GameObject targetActual = distancia1 < distancia2 ? TargetUno : TargetDos;
        //si distancia1 es menor elige Target1, si no elige Target2.

        Vector3 targetPos = targetActual.transform.position;
        Vector3 myPos = transform.position;
        float distancia = Vector3.Distance(targetPos, myPos);
        Vector3 direction = (targetPos - myPos).normalized;
        //Guarda la posición del objetivo y la tuya,
        //luego calcula la dirección normalizada (sin magnitud) de ti hacia él.

        //Logica (ataca solo en el radio izquierdo horizontal )
        bool estaALaDerecha = targetPos.x > myPos.x;

        /*if (Vector3.Distance(targetPos, myPos) < radiusMovement2 && estaALaIzquierda)
            if (distancia < radiusMovement2)
            {
                // Solo ataca si está a la izquierda Y dentro del radio de ataque
                if (estaALaDerecha && distancia < raidusAttack2)
                {
                    if (isAbleToAttack2)
                    {
                        Debug.Log("Atacando a " + targetActual.name);

                        //Espadachin espadachin = targetActual.GetComponent<Espadachin>();
                        //Compita compita = targetActual.GetComponent<Compita>();

                        if (espadachin != null) espadachin.Health -= damage2;
                        if (compita != null) compita.Health -= damage2;

                        isAbleToAttack2 = false;
                        currentTime2 = 0f; // 
                    }
                }
                else
                {   // Se mueve hacia el target (cuando no ataca o target está a la derecha)
                    transform.position += direction * Speed2 * Time.deltaTime;
                }
            }*/
    }
}
