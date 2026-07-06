using UnityEngine;

public class HeallthManager : MonoBehaviour
{
    public float Health = 10;

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

}
