using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public enum ZonaCamara 
    { General,
      ZonaUno,
    }
    public ZonaCamara zonaActual = ZonaCamara.General;

    // esta wuebada esta en el inspector changes in code dont funcking apply 
    //unless they are changed in inspector as well m
    [Header("Clamp General")]
    public float limiteIzquierdoGeneral = -1.0f;
    public float limiteDerechoGeneral = 57f;

    [Header("Clamp Zona Uno")]
    public float limiteIzquierdoUno = -7.8f;
    public float limiteDerechoUno = 11f;

    //need +zones

    private float Speed = 6;
    private float radiusMovement = 0.05f;
    /* evitar que la cámara tiemble o se mueva por micro-movimientos del personaje 
     * (por ejemplo, si el personaje vibra un pixel de un lado a otro estando quietobaja ese valor 
     No actua como estabilzador, actua como Piensa en el sensor de movimiento de una luz de patio
    if movement is strong enough will lock in*/
                
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

    /*camara de m porque todo es mate*/

    public void FollowTarget(GameObject Target)
    {
        Vector3 targetPos = Target.transform.position;
        Vector3 myPos = transform.position;

        Vector3 filterPos = new Vector3(targetPos.x, -0.91f, myPos.z);
        //player z = -10 und myPos z = -10

        if (Vector3.Distance(filterPos, myPos) > radiusMovement)
        {
            Vector3 direction = (filterPos - myPos).normalized;
            transform.position += direction * Speed * Time.deltaTime;
        }

        // Decide qué clamp usar según la posicón actual
        float clampedX = transform.position.x;

        switch (zonaActual)
        {
            case ZonaCamara.General:
                if (clampedX < limiteIzquierdoGeneral)
                {
                    clampedX = limiteIzquierdoGeneral;
                    transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                }
                else if (clampedX > limiteDerechoGeneral)
                {
                    clampedX = limiteDerechoGeneral;
                    transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                }
                break;
            case ZonaCamara.ZonaUno:
                if (clampedX < limiteIzquierdoUno)
                {
                    clampedX = limiteIzquierdoUno;
                    transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                }
                else if (clampedX > limiteDerechoUno)
                {
                    clampedX = limiteDerechoUno;
                    transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
                }
                break;

        }

        /*limitar el movimiento (clamp) dentro de un rango, no congelar ni teletransportar. 
         * Es más simple de mantener.
         * Al tener ZonaActual, necesitamos sostener el argumento ZonaActual" con uno 
         * o muchos triggers collider en escena, ademas de asignar los limites de cada Zona
         * y armar la logica para cda 1.
         */


        /*Lógica de clamp
         * borde_visible_izquierdo = camera.transform.position.x - (mitad del ancho visible)
         * Calcular El ancho visible: 
         * Object: Main Camera 
         * SubObject: Camera
         * Projection >
         * Projection >> Orthographic
         * Size 7.27 (7.27 es la mitad superior, entonces camara completa es size x2 = 14.54)
         * in Game: Aspect ratio 16:9 = 16 / 9 = 1.777
         * formula : ancho_visible = altura_visible * aspect 
         * application : av = 14.54 * 1.777 = 25.84 (aprox)
         * mitad_ancho = 25.84 / 2 = 12.92 (aprox)
         */

        /*
         Tu variable limiteIzquierdoGeneral no es 
         "donde quiero que se vea el borde" 
         sino "donde tiene que estar la camara para que el borde se vea ahi". 
         Como hay un desfase de 11 unidades, tenes que sumarle 11 al numero que realmente queres ver en pantalla.
         
            -12 (borde que quiero ver)
            + 11 (corrección, porque la vista se corre 11 hacia la izquierda) 
            esperás verlo (el "-12" real de tu diseño), confirmado: el offset real es 11
            = -1 */

    }

}
