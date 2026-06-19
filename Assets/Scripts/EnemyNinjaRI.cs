using UnityEngine;

public class EnemyNinjaRI : MonoBehaviour
{
    public float damage;
    public GameObject Target; //add Espadachin
    public EspadachinManager espadachinManager;
    public float Speed;

    public bool isAbleToAttack = true;
    public float radiusMovement;
    public float radiusAttack;

    public float cooldownAtaque = 1.5f;
    public float currentTime;

    public float cooldownMoveD = 0;
    public float currentTimeMoveD = 0.2f;

    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (!isAbleToAttack)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= cooldownAtaque)
            {
                isAbleToAttack = true;
                currentTime = 0f;
            }
        }

        FollowTarget();
    }

    //necesito hacer tests donde uso variables lcoales y globales
    public void FollowTarget()
    {
        Vector3 targetPos = Target.transform.position;
        Vector3 myPos = transform.position;

        Vector3 direction = (targetPos - myPos).normalized;

        // aqui la logica esta mal porque declarar una función global llama a el OBJETO "ESPADACHIN"
        // para la funcion de restar health si la distancia es cercana,
        
        /* PERO para el target de posiciones son solo Target entonces keine AHNUNG 
         */

        //->Si mi distancia entre mi posicion y mi objetivo es menor a mi radio de movimiento lo sigo!
        if (Vector3.Distance(targetPos, myPos) < radiusMovement)
        {
            if (Vector3.Distance(targetPos, myPos) < radiusAttack)
            {
                if (isAbleToAttack)
                {
                    //->GetComponent
                    Debug.LogWarning("attacking");

                    espadachinManager = Target.GetComponent<EspadachinManager>();
                    espadachinManager.Health -= damage; //sigue habiendo errores xd

                    isAbleToAttack = false;
                }
                else
                {
                    transform.position += direction * Speed * Time.deltaTime;
                }
            }
        }
    }

    /*pones un timer que sea por ejemplo 1 y vaya bajando poco a poco, mientras sea mayor que cero
    * hara la funcion de moverse hacia abajo pero cuando ese timer sea menor que 0 hara el target follow que ya hiciste. */
    //deberia ver un video de spawners y a esto le voy a dar vueltas 

    public void MoveDown()
    {
        Vector3 filterPos = new Vector3(0, -1f, 0);
        transform.position += filterPos * Time.deltaTime;

        currentTimeMoveD -= Time.deltaTime;
        if (currentTimeMoveD <= cooldownMoveD)
        {
            FollowTarget();
        }
    }
}


/*
        if (
            Vector3.Distance(targetPos,1) < radiusAttack
            ) {
            {
                transform.position += direction * Speed * Time.deltaTime;
            }
        }
        */






