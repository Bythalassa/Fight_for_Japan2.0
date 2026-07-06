using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Compita : MonoBehaviour
{
    public SpriteRenderer sprite;
    public float speed;
    public float Health;
    private bool isAbleToAttack = false;
    private float damage = 10f;
    public GameObject Target;

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
    private void AttackHolder()
    {

        if (Input.GetKeyDown(KeyCode.X))
    {
        isAbleToAttack = true;
    }

        if (isAbleToAttack)
        {
            Debug.Log("Atacando");
            //Target.GetComponent<Enemy>().Health -= damage;
            isAbleToAttack = false;
        }
    }

}
