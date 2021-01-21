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

    public delegate void OnGameStart();
    public OnGameStart gameStartDel;

    public bool gameStart = false;

    public void StartGame()
    {
        gameStart = true;

        gameStartDel?.Invoke();
    }
}
