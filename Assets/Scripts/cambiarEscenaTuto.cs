using UnityEngine;

public class cambiarEscenaTuto : MonoBehaviour
{
    public bool puedeDestruirse;


    // Update is called once per frame
    void Update()
    {
        if (puedeDestruirse)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Destroy(gameObject);
            }
        }
    }
}
