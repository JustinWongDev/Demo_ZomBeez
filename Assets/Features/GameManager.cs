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

    [Header("UI")] 
    public TextMeshProUGUI t_Jelly;
    
    [Header("General")] 
    public int jellyObtained = 0;
    
    public delegate void OnGameStart();
    public OnGameStart gameStartDel;

    public bool gameStart = false;

    public void StartGame()
    {
        gameStart = true;

        gameStartDel?.Invoke();
    }

    private void Update()
    {
        UpdateText();
    }

    void UpdateText()
    {
        t_Jelly.text = "Jelly: " + jellyObtained;
    }
}
