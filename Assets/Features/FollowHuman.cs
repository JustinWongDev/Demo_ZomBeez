using UnityEngine;

public class FollowHuman : MonoBehaviour
{
    public HumanController human;
    public float distAboveHuman = 30.0f;

    private void Update()
    {
        TrackHuman();
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
}
