using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamButtonController : MonoBehaviour
{
    [SerializeField] private HumanCamButton prefab = null;

    void Start()
    {
        HumanManager.Instance.OnNewHuman += NewButton;
    }

    private void NewButton(HumanController obj)
    {
        HumanCamButton go = Instantiate(prefab, this.transform);
        go.GetComponent<HumanCamButton>().Initialise(obj);
    }
}
