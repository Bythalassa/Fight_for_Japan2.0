using UnityEngine;

public class pMovement : MonoBehaviour
{
    public float speed;

    void Start()
    {
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(x, y, 0);
        direction.Normalize();
        transform.position += direction * speed * Time.deltaTime;
    }
}

