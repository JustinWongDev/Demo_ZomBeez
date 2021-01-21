using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform cam => Camera.main.transform;

    private void Update()
    {
        Face();
    }

    void Face()
    {
        transform.LookAt(cam);
    }
}
