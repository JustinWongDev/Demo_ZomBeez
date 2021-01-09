using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager live;

    public delegate void OnGameStart();
    public OnGameStart gameStartDel;

    public bool gameStart = false;

    private void Awake()
    {
        live = this;
    }

    public void StartGame()
    {
        gameStart = true;

        gameStartDel?.Invoke();
    }
}
