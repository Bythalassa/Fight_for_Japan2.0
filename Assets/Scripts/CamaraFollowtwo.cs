using UnityEngine;
public class CameraFollowtwo : MonoBehaviour
{
    public enum ZonaCamara
    {
        General,
        ZonaUno,
    }
    public ZonaCamara zonaActual = ZonaCamara.General;

    [Header("Clamp General")]
    public float limiteIzquierdoGeneral = -1.0f;
    public float limiteDerechoGeneral = 57f;

    [Header("Clamp Zona Uno")]
    public float limiteIzquierdoUno = -7.8f;
    public float limiteDerechoUno = 11f;

    private float Speed = 6;
    private float radiusMovement = 0.05f;

    public Relevo relevoScript;
    public GameObject compita;
    public GameObject espadachin;

    void Start()
    {
        Debug.Log("CamaraFollow está activo en: " + gameObject.name);
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
        Vector3 myPos = transform.position;
        Vector3 filterPos = new Vector3(targetPos.x, -0.91f, myPos.z);

        if (Vector3.Distance(filterPos, myPos) > radiusMovement)
        {
            Vector3 direction = (filterPos - myPos).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }

        // --- NUEVO: decidir la zona según la posición X del PLAYER, no de la cámara ---
        if (targetPos.x >= limiteIzquierdoUno && targetPos.x <= limiteDerechoUno)
        {
            zonaActual = ZonaCamara.ZonaUno;
        }
        else
        {
            zonaActual = ZonaCamara.General;
        }

        // --- Clamp según la zona ---
        float clampedX = transform.position.x;
        switch (zonaActual)
        {
            case ZonaCamara.General:
                if (clampedX < limiteIzquierdoGeneral)
                    clampedX = limiteIzquierdoGeneral;
                else if (clampedX > limiteDerechoGeneral)
                    clampedX = limiteDerechoGeneral;
                break;

            case ZonaCamara.ZonaUno:
                if (clampedX < limiteIzquierdoUno)
                    clampedX = limiteIzquierdoUno;
                else if (clampedX > limiteDerechoUno)
                    clampedX = limiteDerechoUno;
                break;
        }

        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}