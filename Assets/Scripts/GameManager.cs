using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Se crea un singleton 

    // Se inicializa la variable de puntos en cero 
    public int Points = 0;

    // Se inicializa un nuevo evento para cuando aumenten los puntos pueda "avisar" a los demas componentes
    public UnityEvent onPointsUpdated;

    // Variables encargadas del tiempo de juego
    public float timeOut = 10f;
    public float currentTime = 0f;

    // Enum encargado del estado del juego
    public enum GameState
    {
        Idle,
        InGame,
        GameOver
    }

    // Variable la cual almacenará el estado del juego
    public GameState gameState;

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

    // Funcion encargada de actualizar el tiempo
    private void Update()
    {
        // Se valida que el estado del juego sea 'En partida'
        if(gameState == GameState.InGame)
        {
            // Se cuenta el tiempo entre match utilizando la libreria Time y su funcion deltaTime
            currentTime += Time.deltaTime;

            // Se revisa que el tiempo actual sea mayor que el timeOut para cambiar el estado de la partida
            if(currentTime > timeOut)
            {
                gameState = GameState.GameOver;
            }
        }
    }

    // Se crea metodo que suma los puntos
    public void AddPoint(int newPoints)
    {
        Points += newPoints; // Suma puntos
        onPointsUpdated?.Invoke(); // 
        currentTime = 0f; // reinicia el contador del tiempo para hacer match
    }
}
