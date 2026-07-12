using Unity.Multiplayer.PlayMode;
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
    public GameObject Iconcompita;
    public GameObject Iconespadachin;

    public static Transform CurrentPlayer;

    void Start()
    {
        compita.SetActive(false);
        espadachin.SetActive(true);
        Iconcompita.SetActive(false);
        Iconespadachin.SetActive(true);
        isAvailable = false;
        CurrentPlayer = espadachin.transform;

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
                compita.transform.position = espadachin.transform.position; // dicirle al prof porque no funciona
                compita.SetActive(true);
                espadachin.SetActive(false);
                Iconcompita.SetActive(true);
                Iconespadachin.SetActive(false);
                CurrentPlayer = compita.transform;

            }
            else if (!isAvailable)
            {
                espadachin.transform.position = compita.transform.position; // dicirle al prof porque no funciona
                compita.SetActive(false);
                espadachin.SetActive(true);
                Iconcompita.SetActive(false);
                Iconespadachin.SetActive(true);
                CurrentPlayer = espadachin.transform;

            }

        }
    }
}
