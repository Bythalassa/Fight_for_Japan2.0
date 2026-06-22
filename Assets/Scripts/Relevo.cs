using UnityEngine;
using UnityEngine.InputSystem;


/*Nota de optimización: Haz que en vez de desactivar el gameObject desactive el script de movimiento y
 * ademas cambie la opacidad del sprite a la mitad cuando no esta siendo usado*/

public class Relevo : MonoBehaviour
{
    public bool isAvailable;
    public float timeToRespawn = 1.0f;
    private float timeRespawn; //default 0f
    public GameObject compita;
    public GameObject espadachin;


    void Start()
    {
        compita.SetActive(false);
        espadachin.SetActive(true);
        isAvailable = false;
    }

    void Update()
    {
        LeerInteraccion();

        timeRespawn -= Time.deltaTime;
        if (timeRespawn < 0) timeRespawn = 0;
    }

    private void LeerInteraccion()
    {
        if (timeRespawn > 0) { return; }

        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            timeRespawn = timeToRespawn;
            isAvailable = !isAvailable;

            if (isAvailable)
            {
                compita.SetActive(true);
                espadachin.SetActive(false);
                //add condition (que spawnee en el lugar referencia de el otro <compita y espadachin>) 

            }

            else if (!isAvailable)
            {
                //add condition (que spawnee en el lugar referencia de el otro <compita y espadachin>) 

                compita.SetActive(false);
                espadachin.SetActive(true);
            }

        }
    }
}
