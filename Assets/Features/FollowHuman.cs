using UnityEngine;

public class FollowHuman : MonoBehaviour
{
    public HumanController human;
    public float distAboveHuman = 30.0f;

    public Transform[] cameraPos;
    public enum CameraPos { start, game};
    public CameraPos currentPos = CameraPos.start;

    private void Start()
    {
        transform.position = cameraPos[0].position;
    }

    private void Update()
    {
        //TrackHuman();
    }

    void TrackHuman()
    {
        if (human)
        {
            Vector3 newPos = human.gameObject.transform.position;
            newPos.y += distAboveHuman;
            transform.position = newPos;
        }
    }

    public void PosCameraForGame()
    {
        transform.position = cameraPos[1].position;
    }
}
