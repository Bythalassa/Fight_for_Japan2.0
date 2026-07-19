using UnityEngine;

public class EnemyMill : MonoBehaviour
{

    public GameObject Target;//->si mi target tiene player
    public float Speed;
    public float Health;

    public float damage;

    public float DetectionRadius;
    public float radiusMovement;
    public float raiusAttack;

    public bool isAbleToAttack = true;
    public float currentTime;
    public float MaxTime = 2f;


    void Start()
    {
        Target = GameObject.FindGameObjectWithTag("Player");

    }

    void Update()
    {
        FollowTarget();
        if (!isAbleToAttack)
            TimerToDoSmt();
    }
    public void FollowTarget()
    {
        Vector3 targetPos = Target.transform.position;
        Vector3 myPos = transform.position;

        Vector3 direction = (targetPos - myPos).normalized;

        //->Si mi distancia entre mi posicion y mi objetivo es menor a mi radio de movimiento lo sigo!
        if (Vector3.Distance(targetPos, myPos) < radiusMovement)
        {

            if (Vector3.Distance(targetPos, myPos) < raiusAttack)
            {
                if (isAbleToAttack)
                {
                    //->GetComponent
                    Debug.Log("Atacando");
                    Target.GetComponent<Health>().Vida -= damage;
                    isAbleToAttack = false;
                }
            }
            else
            {
                transform.position += -direction * Speed * Time.deltaTime;
            }
        }
    }
    public void TimerToDoSmt()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= MaxTime)
        {
            //-> ejecutar algo
            isAbleToAttack = true;

            currentTime = 0;
        }
    }
}