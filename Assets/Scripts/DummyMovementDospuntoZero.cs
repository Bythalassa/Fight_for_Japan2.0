using System.Collections;
using UnityEngine;

public enum DummyState
{
    IdleAfterSpawn,
    WalkBehind,
    Approach,
    WaitPhase,
    Camping,
    CrossRange,
    ExtremeSway,
    Recovery,
    Attacking,
    Dead
}

public class DummyMovementDospuntoZero : MonoBehaviour
{
    private GameObject playerTarget;
    private Animator anim;
    private Health Vida; 

    [Header("Movimiento general")]
    public float Maxspeed = 3f;
    public float radiusMovement = 3f; //(medir que chota con esto pq wtf)
    public float sideOffset = 20f; //same here
    private float side; // -1 izquierda / 1 derecha respecto al player

    //var para la funcion de detectar camara &&  IdleAfterSpawn case
    private bool isOnCamRange = false;
    
    
    //lista de Changeable Vars je nach case context
    //TimetoIdle
    private float speed;//testear si puedo usar este speed para todo ekisdfeeee
    private float currentTime; 
    private float MaxTime = 3f;


    [SerializeField] private RuntimeAnimatorController controladorThisEnemy;

    [Header("Estado actual (solo lectura, para debug)")]
    public DummyState state = DummyState.IdleAfterSpawn;

    void Start()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
        Vida = GetComponent<Health>();
        isOnCamRange = GetComponent<EnemyCameraCheck>();


    }

    void Update()
    {

        //main updated position
        Vector3 targetPos = playerTarget.transform.position;
        Vector3 myPos = transform.position;
        Vector3 direction = (targetPos - myPos).normalized;

        switch (state)
        {
            case DummyState.IdleAfterSpawn :
                transform.position += direction * Maxspeed * Time.deltaTime; 
                
                if (isOnCamRange)
                {
                    currentTime += Time.deltaTime;
                    if (currentTime >= MaxTime)
                    {
                        //-> ejecutar algo
                        speed = 0f; // ya no c mueve
                        Maxspeed = speed;
                        anim.SetTrigger("Idle");

                        currentTime = 0;
                    }
                    else if (currentTime == 0 ) { state = DummyState.Approach; }
                }
                break;
            case DummyState.WalkBehind:
                {

                }
                break;
            case DummyState.Approach:
                {

                }
                break;
            case DummyState.WaitPhase:
                {

                }
                break;
            case DummyState.Camping:
                {

                }
                break;
            case DummyState.CrossRange:
                {

                }
                break;
            case DummyState.ExtremeSway:
                {

                }
                break;
            case DummyState.Recovery:
                {

                }
                break;
            case DummyState.Attacking:
                {

                }
                break;
            case DummyState.Dead:
                {

                }
                break;
            default:
                break;
        }
    }




 }
