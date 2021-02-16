using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CamState currentState = CamState.Menu;

    public enum CamState
    {
        Menu,
        Follow
    }

    public bool CanFollowHuman()
    {
        return currentState == CamState.Follow;
    }

    public void SwitchCamState(CamState newState)
    {
        if (newState == CamState.Menu)
        {
            FindObjectOfType<CinemachineController>().GetComponent<CinemachineController>().enabled = true;
            FindObjectOfType<HumanCam>().GetComponent<HumanCam>().enabled = false;
            return;
        }        
        
        if (newState == CamState.Follow)
        {
            FindObjectOfType<CinemachineController>().GetComponent<CinemachineController>().enabled = false;
            FindObjectOfType<HumanCam>().GetComponent<HumanCam>().enabled = true;
        }
    }
}
