using System;
using UnityEngine;
using TMPro;

public class Depot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _jellyText = null;
    [SerializeField] private DepotAnimController animController = null;
    
    private int _jellyAmount = 0;

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        _jellyText.text = "Jelly: " + _jellyAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<HumanController>().HasJelly)
        {
            other.GetComponent<HumanController>()._hasJelly = false;
            _jellyAmount++;
            animController.PlayDepositAnim();
        }
    }
}
