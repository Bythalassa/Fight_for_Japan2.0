using UnityEngine;

public class ZonaActualHandler : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public CameraFollow.ZonaCamara zonaAssigned;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered trigger, changing camera zone to: " + zonaAssigned);  
            cameraFollow.zonaActual = zonaAssigned;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player exited trigger, changing camera zone to: General");
            cameraFollow.zonaActual = CameraFollow.ZonaCamara.General;
        }
    }
}
/*
OnTriggerStay2D sirve para cuando necesitas revisar o actualizar algo continuamente, 
frame a frame, mientras el jugador sigue dentro del trigger. 

Antes lo necesitabas porque FollowTargetTwo tenía que recalcular constantemente 
la posición del jugador para mover la cámara en tiempo real.

Ahora, el trigger solo hace una asignación simple:

Esto es una operación de "set and forget" (asignar y olvidar).
Una vez que zonaActual cambia a ZonaUno, esa variable se queda así hasta que 
algo más la cambie — no necesita repetirse en cada frame porque no es un valor que 
"se revierta" solo. CameraFollow.Update() ya se encarga de leer esa variable constantemente
por su cuenta, en su propio bucle. El trigger no necesita "recordarle" nada más.

5. ¿Las capas (Layers) están habilitadas para colisionar entre sí?
Andá a Edit > Project Settings > Physics 2D, mirá la matriz de colisión (Layer Collision Matrix) al final.
Si el layer del trigger y el layer del Player tienen la casilla desmarcada entre ellos, nunca van a detectarse,
sin importar que todo lo demás esté bien configurado.
 */