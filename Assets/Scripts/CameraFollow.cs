using UnityEngine;

/*Nota de optimización: usar un algo que le pasa una posicion especial cuando el player pelea para que no se mueva
y lo deje tranquilo dando sus golpes*/
public class CameraFollow : MonoBehaviour
{
    private float Speed = 6;
    public Relevo relevoScript;

    public GameObject compita;
    public GameObject espadachin;

    private float radiusMovement = 1.7f;

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

        Vector3 myPos = transform.position;
        if (Vector3.Distance(filterPos, myPos) > radiusMovement)

        {
            Vector3 direction = (filterPos - myPos).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }
    }
}
