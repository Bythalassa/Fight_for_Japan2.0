using UnityEngine;

public class Espadachin : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float Health;
    public float Speed;

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

    //add attacks

}
