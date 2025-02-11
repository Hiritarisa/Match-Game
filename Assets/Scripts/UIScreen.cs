using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIScreen : MonoBehaviour
{
    // Propiedades para las transiciones de la pantalla
    public RectTransform containerRect;
    public CanvasGroup containerCanvas;
    public Image background;
    public GameManager.GameState visibleState;
    public float transitionTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated);
        bool initialState = GameManager.Instance.gameState == visibleState;
        background.enabled = initialState;
        containerRect.gameObject.SetActive(initialState);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if(newState == visibleState){
            ShowScreen();
        }else{
            HideScreen();
        }
    }

    private void HideScreen()
    {
        // Animacion de fondo
        var bgColor = background.color;
        bgColor.a = 0;
        background.DOColor(bgColor, transitionTime * 0.5f);

        // Animacion de contenedor
        containerCanvas.alpha = 1;
        containerRect.anchoredPosition = Vector2.zero;
        containerCanvas.DOFade(0, transitionTime * 0.5f);
        containerRect.DOAnchorPos(new Vector2(0, -100), transitionTime * 0.5f).onComplete = () => { // Funcion de flecha
            background.enabled = false;
            containerRect.gameObject.SetActive(false);
        };
    }

    private void ShowScreen()
    {
        // Activar los elementos
        background.enabled = true;
        containerRect.gameObject.SetActive(true);

        // Animacion de fondo
        var bgColor = background.color;
        bgColor.a = 0;
        background.color = bgColor;
        bgColor.a = 1;
        background.DOColor(bgColor, transitionTime);

        // Animacion del contenedor
        containerCanvas.alpha = 0;
        containerRect.anchoredPosition = new Vector2(0,100);
        containerCanvas.DOFade(1, transitionTime);
        containerRect.DOAnchorPos(Vector2.zero, transitionTime);
    }
}
