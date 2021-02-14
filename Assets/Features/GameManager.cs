using System;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Static Singleton
    public static GameManager live;
    private void Awake()
    {
        live = this;
    }
    #endregion

    public event Action OnGameStart;
    

    private bool gameStart = false;

    public void StartGame()
    {
        gameStart = true;

        OnGameStart?.Invoke();
    }

    public bool hasGameStarted()
    {
        return gameStart;
    }
}
