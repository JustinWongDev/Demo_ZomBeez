using UnityEngine;

public class BeeVisualiser : MonoBehaviour
{
    [SerializeField] private Material mat_normal;
    [SerializeField] private Material mat_Detect;
    [SerializeField] private Material mat_Forage;
    [SerializeField] private Material mat_Attack;

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
