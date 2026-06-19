using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CamaraFollow : MonoBehaviour
{
    // usar un algo que le pasa una posicion especial cuando el player pelea para que no se mueva
    // y lo deje tranquilo dando sus golpes

    private float Speed = 6;
    public Relevo relevoScript;

    public GameObject compita;
    public GameObject espadachin;

    private float radiusMovement = 1.7f;
    //que es 


    void Start()
    {

    }

    void Update()
    {
        
        if (!relevoScript.isAvailable)
        {
            FollowTarget(espadachin);
        }
        else if (relevoScript.isAvailable)
        {
            FollowTarget(compita);
        }
    }

    public void FollowTarget(GameObject Target)
    {
        Vector3 targetPos = Target.transform.position;
        Vector3 filterPos = new Vector3(targetPos.x, 0f, -1);
        //kge-----posición transform filtrando z : porque unity trabaja todo 3d y necesita que
        //lacamara este atrás de los sprites (-1 o menor)

        //usar la logica de igual a Numero para limite del mapa : Igualo el numero de x a la posicion limite
        //ejemplo limite de la izquierda:  Posicion transform x = -6.5 entonces cuando if x < -6.5 { x = -6.5 }
        //derecha seria positivo pe 


        Vector3 myPos = transform.position;
        if (Vector3.Distance(filterPos, myPos) > radiusMovement)
            //SI ESTA LEJOS debe seguir 
        { 
            Vector3 direction = (filterPos - myPos).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }        
    }
}

