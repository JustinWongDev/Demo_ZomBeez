using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableInGame : MonoBehaviour
{
    void Start()
    {
        GameManager.live.OnGameStart += EnableOnGameStart;
        this.gameObject.SetActive(false);
    }

    void EnableOnGameStart()
    {
        this.gameObject.SetActive(true);
    }
}
