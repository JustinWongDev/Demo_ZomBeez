using UnityEngine;

public class BeeVisualiser : MonoBehaviour
{
    [SerializeField] private Material mat_normal = null;
    [SerializeField] private Material mat_Detect = null;
    [SerializeField] private Material mat_Forage = null;
    [SerializeField] private Material mat_Attack = null;

    private TrailRenderer trail => GetComponent<TrailRenderer>();
    private Worker bee => GetComponent<Worker>();

    void Update()
    {
        VisualiseDetectedHuman();
    }

    void VisualiseDetectedHuman()
    {
        switch (bee.beeBehaviour)
        {
            case Worker.BeeBehaviours.Attack:
                trail.material = mat_Attack;
                break;
            case Worker.BeeBehaviours.Forage:
                trail.material = mat_Forage;
                break;
            case Worker.BeeBehaviours.Idle:
                trail.material = mat_normal;
                break;
            case Worker.BeeBehaviours.Scout:
                trail.material = bee.NewHuman ? mat_Detect : mat_normal;
                break;
        }
    }
}
