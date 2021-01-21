using UnityEngine;

public class BeeVisualiser : MonoBehaviour
{
    [SerializeField] private Material mat_normal;
    [SerializeField] private Material mat_Detect;

    private TrailRenderer trail => GetComponent<TrailRenderer>();
    private Worker bee => GetComponent<Worker>();

    void Update()
    {
        VisualiseDetectedHuman();
    }

    void VisualiseDetectedHuman()
    {
        trail.material = bee.NewHuman ? mat_Detect : mat_normal;
    }
}
