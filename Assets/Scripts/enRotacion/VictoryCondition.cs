using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryCondition : MonoBehaviour
{

    public GameObject GameObject;

    void Start()
    {
      

    }


    void Update()
    {

        GameObject = GameObject.FindGameObjectWithTag("Enemy");
        
        if (GameObject == null)
        {
            SceneManager.LoadScene("Victory");
            //load Victory scene
        }



    }


}
