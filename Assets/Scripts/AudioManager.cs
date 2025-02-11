using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }else{
            Destroy(gameObject);
        }
    }

    // Sonidos
    public AudioClip moveSFX;
    public AudioClip missSFX;
    public AudioClip matchSFX;
    public AudioClip gameOverSFX;

    // Referencia encargada de reproducir los sonidos
    public AudioSource SfxSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.onPointsUpdated.AddListener(PointsUpdated);
        GameManager.Instance.OnGameStateUpdated.AddListener(GameStateUpdated);
    }

    private void OnDestroy() {
        GameManager.Instance.onPointsUpdated.RemoveListener(PointsUpdated);
        GameManager.Instance.OnGameStateUpdated.RemoveListener(GameStateUpdated);
    }

    private void GameStateUpdated(GameManager.GameState newState)
    {
        if(newState == GameManager.GameState.GameOver){
            SfxSource.PlayOneShot(gameOverSFX);
        }

        if (newState == GameManager.GameState.InGame){
            SfxSource.PlayOneShot(matchSFX);
        }
    }

    private void PointsUpdated()
    {
        SfxSource.PlayOneShot(matchSFX);
    }

    public void Move()
    {
        SfxSource.PlayOneShot(moveSFX);
    }

    public void Miss()
    {
        SfxSource.PlayOneShot(missSFX);
    }
}
