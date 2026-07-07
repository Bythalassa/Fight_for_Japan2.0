using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    // Estado actual de la camara.
    // Libre = sigue al jugador (espadachin/compita) como antes.
    // PosicionX = la camara se ancla a un punto fijo de combate.
    public enum CameraState
    {
        Libre,
        Posicion1,
        Posicion2,
        Posicion3
    }

    [Header("Referencias generales")]
    public Relevo relevoScript;
    public GameObject compita;
    public GameObject espadachin;
    public GameObject player;          // Objeto con tag Player / HealthManager

    [Header("Posiciones fijas de combate")]
    public GameObject posicion1;
    public GameObject posicion2;
    public GameObject posicion3;

    [Header("Configuracion de movimiento")]
    public float Speed = 6f;
    public float radiusMovement = 1.7f;
    public float radiusDeteccionZona = 2f; // cercania del personaje > 

    private HeallthManager healthManager;

    private CameraState estadoActual = CameraState.Libre;

    void Start()
    {
        if (player != null)
            healthManager = player.GetComponent<HeallthManager>();
    }

    void Update()
    {
        estadoActual = DeterminarEstado();

        switch (estadoActual)
        {
            case CameraState.Posicion1:
                FollowTarget(posicion1);
                break;

            case CameraState.Posicion2:
                FollowTarget(posicion2);
                break;

            case CameraState.Posicion3:
                FollowTarget(posicion3);
                break;

            case CameraState.Libre:
            default:
                SeguimientoPrincipal();
                break;
        }
    }

    // la lógica que tiene que aplicarse en cada estado

    private CameraState DeterminarEstado()
    {
        // Sin pelea -> siempre modo libre, sin importar donde este el jugador
        // 
        if (healthManager == null || !healthManager.OnPelea)
            return CameraState.Libre;

        // En pelea -> revisamos si el jugador esta cerca de alguna posicion fija
        if (posicion1 != null && EstaCerca(player.transform.position, posicion1.transform.position))
            return CameraState.Posicion1;

        if (posicion2 != null && EstaCerca(player.transform.position, posicion2.transform.position))
            return CameraState.Posicion2;

        if (posicion3 != null && EstaCerca(player.transform.position, posicion3.transform.position))
            return CameraState.Posicion3;

        // En pelea pero lejos de cualquier posicion fija -> sigue en modo libre
        return CameraState.Libre;
    }

    private bool EstaCerca(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b) <= radiusDeteccionZona;
    }

    // Tu logica original de espadachin/compita, ahora aislada en su propio metodo
    private void SeguimientoPrincipal()
    {
        if (!relevoScript.isAvailable)
            FollowTarget(espadachin);
        else
            FollowTarget(compita);
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