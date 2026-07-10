using System.Collections.Generic;
using UnityEngine;

public class PassiveRadius : MonoBehaviour
{
    // Lista centralizada de enemigos dentro del rango
    public List<DummyMovementDospuntoZero> targets = new List<DummyMovementDospuntoZero>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Verificamos si el objeto tiene el script del enemigo
            if (collision.TryGetComponent<DummyMovementDospuntoZero>(out DummyMovementDospuntoZero enemy)) //agrega la etiqueta enemy a todo el q tiene el script necesitado
            {
                if (!targets.Contains(enemy))
                {
                    targets.Add(enemy);
                    ActualizarCountOnPRangeEnemigos();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent<DummyMovementDospuntoZero>(out DummyMovementDospuntoZero enemy))
            {
                if (targets.Contains(enemy))
                {
                    targets.Remove(enemy);
                    ActualizarCountOnPRangeEnemigos();
                }
            }
        }
    }

    public void ActualizarCountOnPRangeEnemigos()
    {
        // Si hay 5 o más enemigos en la lista, cambian de estado
        bool hayCincoE = targets.Count >= 5;
        //ejemplos logs
        //Si tienes 3 enemigos en la lista: 3 >= 5 es Falso.
        //Si tienes 5 enemigos en la lista: 5 >= 5 es Verdadero.

        foreach (var enemy in targets)
        {
            enemy.FiveEnemiesOnRange = hayCincoE;
            //se actualiza cada add o resta de enemy creado por la lista de PassiveRadious
        }
    }
}