using UnityEngine;
using TMPro;

public class TutorialTextController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _t_Title = null;
    [SerializeField] private TextMeshProUGUI _t_Desc = null;

    [SerializeField] private string[] _listTitle = null;
    [SerializeField] private string[] _listDesc = null;

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
