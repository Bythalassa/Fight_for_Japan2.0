using UnityEngine;
using UnityEngine.InputSystem;

public class Relevo : MonoBehaviour
{
    //se petea porque.

    public bool isAvailable;
    public float timeToRespawn = 1.0f;
    private float timeRespawn; //default 0f
    public GameObject compita;
    public GameObject espadachin;

    private void Start()
    {

        compita.SetActive(false); 
        espadachin.SetActive(true);
        isAvailable = false;
    }

    //Añadir en espadachin: (si compita.enable = true entonces Espadachin.enable false)
    private void Update()
    {
        LeerInteraccion();

        timeRespawn -= Time.deltaTime;
        if (timeRespawn < 0 ) timeRespawn = 0;
    }

    private void LeerInteraccion()
    {
        if ( timeRespawn > 0 ) { return; }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {

            //c# = está definido como asociativo por la derecha && El script declara isAvailable = false
            //luego de presionar de K se actualiza el valor de la variable a = falase.
            //por lo tanto identifica isAvailable y ahora lee de izquierda a derecha  
            //es como forzar preparar limonada y servir un vaso 

            // Flujo: spawn + basic atack golpe 
            // Cooldown y como hacer q el script funciona pa todos los 

            //haz que en vez de desactivar el gameObject desactive el script de movimiento y
            //ademas cambie la opacidad del sprite a la mitad cuando no esta siendo usado

            timeRespawn = timeToRespawn;
            isAvailable = !isAvailable;

            if (isAvailable)
            {
                //add condition (que spawnee en el lugar referencia de el otro <compita y espadachin>) 

                compita.SetActive(true);
                espadachin.SetActive(false);
            }

            else if (!isAvailable)
            {
                //add condition (que spawnee en el lugar referencia de el otro <compita y espadachin>) 

                compita.SetActive(false);
                espadachin.SetActive(true);
            }
            
        }
    }



    // recomendaciones para esta logica optimizada: 
    //Compita, tener una referencia pública al SpriteRenderer de Espadachin, y viceversa
    //El relevo contradictorio — cuando compita.enabled = true, espadachin.enabled debe ser false
    //y viceversa. Siempre opuestos.







}
