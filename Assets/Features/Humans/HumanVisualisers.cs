using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HumanVisualisers : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _aiState = null;
    [SerializeField]
    private Image _hp = null;
    [SerializeField]
    private TextMeshProUGUI _brains = null;
    [SerializeField] private Image _jelly = null;
    [SerializeField] private Color[] _colours = null;

    private HumanBrain _brain => GetComponent<HumanBrain>();
    private HumanController _human => GetComponent<HumanController>();

    private void Update()
    {
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
      
        //AI state
        if(_brain.CurrentAIState != null)
            _aiState.text = _brain.CurrentAIState.ToString();
        
        //HP
        _hp.fillAmount = _human.Settings.Health / 100;
        
        //Brains
        _brains.text = _human.Settings.Brains.ToString();
        
        //Jelly
        _jelly.color = _human.HasJelly ? _colours[1] : _colours[0];
    }
}
