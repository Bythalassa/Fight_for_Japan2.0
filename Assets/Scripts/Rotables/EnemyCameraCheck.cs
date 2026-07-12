using UnityEngine;

public class EnemyCameraCheck : MonoBehaviour
{
    public Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    // Devuelve true si el enemigo está dentro de los límites visibles de la cámara
    public bool IsInsideCameraBounds()
    {
         if (mainCamera == null ) return false;
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        /*WorldToViewportPoint - > Convierte la posición del enemigo al espacio de "viewport" de la cámara 
 * (valores de 0 a 1 en X e Y si está dentro del campo de visión)*/

        bool isVisible = viewportPos.z > 0 && // que esté delante de la cámara, no detrás
                          viewportPos.x >= 0 && viewportPos.x <= 1 &&
                          viewportPos.y >= 0 && viewportPos.y <= 1;

        return isVisible;
    }

}