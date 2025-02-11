using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    public int displayedPoints = 0; // puntos a visualizar
    public TextMeshProUGUI pointsUI; // UI a 

    void Start()
    {
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated); // se 'suscribe' a un elemento
    }

    private void OnDestroy() {
        GameManager.Instance.OnGameStateUpdated.RemoveListener(GameStateUpdated); // Elimina la 'suscripcion' a un elemento
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if(newState == GameManager.GameState.GameOver){
            displayedPoints = 0;
            StartCoroutine(DisplayPointsCoroutine());
        }
    }

    IEnumerator DisplayPointsCoroutine()
    {
        while(displayedPoints < GameManager.Instance.Points){
            displayedPoints++;
            pointsUI.text = displayedPoints.ToString();
            yield return new WaitForFixedUpdate();
        }
        displayedPoints = GameManager.Instance.Points;
        pointsUI.text = displayedPoints.ToString();
        yield return null;
    }

    public void PlayAgainBtnClicked(){
        GameManager.Instance.RestartGame();
    }

    public void ExitGameBtnClicked(){
        GameManager.Instance.ExitGame();
    }
}
