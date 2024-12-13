using UnityEngine;
using TMPro;
using System.Collections;

public class UIPoints : MonoBehaviour
{
    int displayedPoints = 0; // Variable que inicializa los puntos visuales en cero
    public TextMeshProUGUI pointsLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Se crea un listener para la funcion onPointsUpdate del Game manager
        GameManager.Instance.onPointsUpdated.AddListener(updatePoints);
    }

    // Se crea la funcion que llamara una coroutine la cual actualizara los puntos secuencialmente y no de inmediato
    void updatePoints()
    {
        StartCoroutine(updatePointsCoroutine());
    }

    // Coroutine encargada de actualizar los puntos
    IEnumerator updatePointsCoroutine()
    {
        while (displayedPoints < GameManager.Instance.Points)
        {
            displayedPoints++;
            pointsLabel.text = displayedPoints.ToString();
            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }
}
