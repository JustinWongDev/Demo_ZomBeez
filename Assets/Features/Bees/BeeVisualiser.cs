using UnityEngine;

public class BeeVisualiser : MonoBehaviour
{
    [SerializeField] private Material mat_normal = null;
    [SerializeField] private Material mat_Detect = null;
    [SerializeField] private Material mat_Forage = null;
    [SerializeField] private Material mat_Attack = null;

    private TrailRenderer trail => GetComponent<TrailRenderer>();
    private BeeController bee => GetComponent<BeeController>();
    private BeeBrain brain => GetComponent<BeeBrain>();
    private BeeResources resources => GetComponent<BeeResources>();

    void Update()
    {
        VisualiseDetectedHuman();
    }

    void VisualiseDetectedHuman()
    {
        if (bee.CurrentAIState.GetType() == typeof(BeeAttack))
        {
            trail.material = mat_Attack;
            return;
        }
        if (bee.CurrentAIState.GetType() == typeof(BeeForage))
        {
            trail.material = mat_Forage;
            return;
        }
        if (bee.CurrentAIState.GetType() == typeof(BeeIdle))
        {
            trail.material = mat_normal;
            return;
        }
        if (bee.CurrentAIState.GetType() == typeof(BeeScout))
        {
            trail.material = resources.NewHuman ? mat_Detect : mat_normal;
            return;
        }
    }
}
