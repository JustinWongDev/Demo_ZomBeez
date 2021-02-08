using UnityEngine;
using TMPro;

public class TutorialTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _t_Title;
    [SerializeField] private TextMeshProUGUI _t_Desc;

    [SerializeField] private string[] _listTitle;
    [SerializeField] private string[] _listDesc;

    private void OnEnable()
    {
        SwitchTutorialText(0);
    }

    public void SwitchTutorialText(int index)
    {
        _t_Title.text = _listTitle[index];
        _t_Desc.text = _listDesc[index];
    }
}
