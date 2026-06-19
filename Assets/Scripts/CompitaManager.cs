using UnityEngine;
using UnityEngine.InputSystem;


public class CompitaManager : MonoBehaviour
{
    //Debug.Log() en cada paso clave para ver qué valor tiene cada variable en cada momento.
    //le falta darle gravedad a la mecanica de caminata de ambos players para lograr el game feel + ataques con pacing
    public SpriteRenderer sprite;
    public float speed;
    public float Health;

    private void Start()
    {

    } 

    private void Update()
    {
        CompitaMoves();

    }

    /*customizar :
    add compita.enable && accion de ataque 1 (do damage) && hacia la direccion presionada
    limitar boton de direcciones a arrows*/

    private void CompitaMoves()
    {

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, y, 0);
        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;
        //limitar movimiento a arrows. 

    }

}
