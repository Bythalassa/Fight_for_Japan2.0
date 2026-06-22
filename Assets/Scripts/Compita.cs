using UnityEngine;

public class Compita : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float speed;
    public float Health;

    void Start()
    {
        
    }

    void Update()
    {
        CompitaMoves();
    }
    private void CompitaMoves()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(x, y, 0);
        direction.Normalize();

        transform.position += direction * speed * Time.deltaTime;
    }

    //add attacks

}
