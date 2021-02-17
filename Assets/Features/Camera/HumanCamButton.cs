using UnityEngine;
using TMPro;

public class HumanCamButton : MonoBehaviour
{
    private HumanController human = null;

    public void Initialise(HumanController human)
    {
        this.human = human;
        GetComponentInChildren<TextMeshProUGUI>().text = human.name;
    }

    public void SetTargetToThis()
    {
        CinemachineController.Instance.SetTargetHuman(this.human);
    }

    public void SetTargetToDefault()
    {
        CinemachineController.Instance.SetTargetToMiddle();
    }
}
