using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public enum CameraState
    {
        Libre,
        Posicion1,
        Posicion2,
        Posicion3,
        Posicion4,
        Posicion5,
        Posicion6,
        Posicion7,
        Posicion8,
        Posicion9,
        Posicion10,
        Posicion11,
        Posicion12,
        Posicion13,
        Posicion14,
        Posicion15,
        Posicion16,
    }

    [Header("Referencias generales")]
    public Relevo relevoScript;
    public GameObject compita;
    public GameObject espadachin;

    [Header("Posiciones fijas de combate")]
    public GameObject posicion1;
    public GameObject posicion2;
    public GameObject posicion3;
    public GameObject posicion4;
    public GameObject posicion5;
    public GameObject posicion6;
    public GameObject posicion7;
    public GameObject posicion8;
    public GameObject posicion9;
    public GameObject posicion10;
    public GameObject posicion11;
    public GameObject posicion12;
    public GameObject posicion13;
    public GameObject posicion14;
    public GameObject posicion15;
    public GameObject posicion16;

    [Header("Configuracion de movimiento")]
    public float Speed = 3.5f;
    public float radiusMovement = 1.7f;
    public float radiusDeteccionZona = 0.5f; // que tan cerca debe estar el jugador de una posicion para activarla
    //falta añadir los limites generales de la cámara

    private CameraState estadoActual = CameraState.Libre;

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

            case CameraState.Posicion4:
                FollowTarget(posicion4);
                break;

            case CameraState.Posicion5:
                FollowTarget(posicion5);
                break;

            case CameraState.Posicion6:
                FollowTarget(posicion6);
                break;

            case CameraState.Posicion7:
                FollowTarget(posicion7);
                break;

            case CameraState.Posicion8:
                FollowTarget(posicion8);
                break;

            case CameraState.Posicion9:
                FollowTarget(posicion9);
                break;

            case CameraState.Posicion10:
                FollowTarget(posicion10);
                break;

            case CameraState.Posicion11:
                FollowTarget(posicion11);
                break;  

            case CameraState.Posicion12:
                FollowTarget(posicion12);
                break;

            case CameraState.Posicion13:
                FollowTarget(posicion13);
                break;

            case CameraState.Posicion14:
                FollowTarget(posicion14);
                break;

            case CameraState.Posicion15:
                FollowTarget(posicion15);
                break;

            case CameraState.Posicion16:
                FollowTarget(posicion16);
                break;

            case CameraState.Libre:
            default:
                SeguimientoPrincipal();
                break;
        }
    }


    private GameObject JugadorActivo()
    {
        if (espadachin.CompareTag("Player"))
            return espadachin;

        return compita;
    }


    private CameraState DeterminarEstado()
    {
        //->  default / default 2 es una copia replicaada
        if (!HayEnemigoEnPelea())
        { return CameraState.Libre; }

        GameObject jugadorActivo = JugadorActivo();

        // En pelea ->condiciones del jugador relativas a las posiciones fijas en escena
        if (posicion1 != null && EstaCerca(jugadorActivo.transform.position, posicion1.transform.position))
        { return CameraState.Posicion1; }

        if (posicion2 != null && EstaCerca(jugadorActivo.transform.position, posicion2.transform.position))
        { return CameraState.Posicion2; }

        if (posicion3 != null && EstaCerca(jugadorActivo.transform.position, posicion3.transform.position))
        { return CameraState.Posicion3; }

        if (posicion4 != null && EstaCerca(jugadorActivo.transform.position, posicion4.transform.position))
        { return CameraState.Posicion4; }

        if (posicion5 != null && EstaCerca(jugadorActivo.transform.position, posicion5.transform.position))
        { return CameraState.Posicion5; }

        if (posicion6 != null && EstaCerca(jugadorActivo.transform.position, posicion6.transform.position))
        { return CameraState.Posicion6; }

        if (posicion7 != null && EstaCerca(jugadorActivo.transform.position, posicion7.transform.position))
        { return CameraState.Posicion7; }

        if (posicion8 != null && EstaCerca(jugadorActivo.transform.position, posicion8.transform.position))
        { return CameraState.Posicion8; }

        if (posicion9 != null && EstaCerca(jugadorActivo.transform.position, posicion9.transform.position))
        { return CameraState.Posicion9; }

        if (posicion10 != null && EstaCerca(jugadorActivo.transform.position, posicion10.transform.position))
        { return CameraState.Posicion10; }

        if (posicion11 != null && EstaCerca(jugadorActivo.transform.position, posicion11.transform.position))
        { return CameraState.Posicion11; }

        if (posicion12 != null && EstaCerca(jugadorActivo.transform.position, posicion12.transform.position))
        { return CameraState.Posicion12; }

        if (posicion13 != null && EstaCerca(jugadorActivo.transform.position, posicion13.transform.position))
        { return CameraState.Posicion13; }

        if (posicion14 != null && EstaCerca(jugadorActivo.transform.position, posicion14.transform.position))
        { return CameraState.Posicion14; }

        if (posicion15 != null && EstaCerca(jugadorActivo.transform.position, posicion15.transform.position))
        { return CameraState.Posicion15; }

        if (posicion16 != null && EstaCerca(jugadorActivo.transform.position, posicion16.transform.position))
        { return CameraState.Posicion16; }

        return CameraState.Libre;
        //porque hay q poner siempre uun return al final q paja 
    }

    private bool HayEnemigoEnPelea() //función que revisa si hay enemigos en pelea
    {
        GameObject[] enemigos = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemigo in enemigos)
        {
            HeallthManager hm = enemigo.GetComponent<HeallthManager>();
            if (hm.OnPelea)
            { return true; }
        }

        return false;
    }

    private bool EstaCerca(Vector3 a, Vector3 b)
    {
        return Vector3.Distance(a, b) <= radiusDeteccionZona;
    }

    //  logica original de espadachin/compita
    private void SeguimientoPrincipal()
    {
        if (!relevoScript.isAvailable)
            FollowTarget(espadachin);
        else
            FollowTarget(compita);
    }

    public void FollowTarget(GameObject Target) //  core ><
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