using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CamState currentState = CamState.Menu;

    public enum CamState
    {
        Menu,
        Follow
    }
}
