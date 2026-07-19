using UnityEngine;
using UnityEngine.UI;

public class CanvasLife : MonoBehaviour
{

    public Image RellenoVida;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ActualizarBarra(float Vida, float maxVida)
    {
        RellenoVida.fillAmount = Vida / maxVida;


    }



}
