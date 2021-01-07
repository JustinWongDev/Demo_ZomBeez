using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager live;

    public delegate void OnGameStart();
    public OnGameStart gameStartDel;

    private void Awake()
    {
        live = this;
    }

    public void StartGame()
    {
        gameStartDel?.Invoke();
    }
}
