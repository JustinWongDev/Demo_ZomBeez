using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.live.OnGameStart += DisableOnGameStart;
    }

    void DisableOnGameStart()
    {
        this.gameObject.SetActive(false);
    }
}
