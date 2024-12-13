using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Se crea un singleton 

    // Se crea la logica para instanciar el Singleton desde donde sea llamado
    private void Awake(){
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Se inicializa la variable de puntos en cero 
    public int Points = 0;

    // Se inicializa un nuevo evento para cuando aumenten los puntos pueda "avisar" a los demas componentes
    public UnityEvent onPointsUpdated;


    // Se crea metodo que suma los puntos
    public void AddPoint(int newPoints)
    {
        Points += newPoints;
        onPointsUpdated?.Invoke();
    }
}
