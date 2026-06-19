using UnityEngine;

public class EspadachinManager : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float Health;
    public float Speed;

    //player necesita gravedad de movimiento tipo Fight N Range + animacion con la referencia de esos monos 

    //El salto se elimina porque, como en el juego Fight*N Range el 
    //Player tiene ataques con dinamismo, no necesita saltar, solo necesita dinamismo de tipo saltos y giros. 
    //Respecto a alcanzar la lampara: el player ataca mirando hacia la pared, no necesarimanete cerca a la pared. 
    //Una espada Se clava en el tope de la lampara, la lampara cae al suelo y se carga el power up sin necesidad de recogerla,
    //lamapara respawn arreglada en su poscicion inicial 
    //pero esto es desde un ataque  

    void Start()
    {

    }

    void Update()
    {
        EspadachinMoves();
 
    }

    void EspadachinMoves()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, y, 0);
        direction.Normalize();

        transform.position += direction * Speed * Time.deltaTime;
    }

    //agregar funcion de ataque melee y hacer el ataque con KEY.X

    //EL ATAQUE ES CON ESPADA

}