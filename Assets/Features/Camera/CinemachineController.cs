using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private CinemachineVirtualCamera[] camsBasic = null;    //0 menu; 1 default game
    [SerializeField] private CinemachineVirtualCamera[] camsTutorial = null; //0 hive; 1 depot; 2 caches; book
    [SerializeField] private CinemachineVirtualCamera camHuman = null;       //0 hive; 1 depot; 2 caches; book

    public void SetCamToHuman()
    {
        camHuman.Priority = 10;
        camHuman.m_LookAt = FindObjectOfType<HumanController>().transform;

        //Security camera view
        // camHuman.Priority = 10;
        // camHuman.m_LookAt = FindObjectOfType<HumanController>().transform;
    }

    public void SwitchToCam(int index)
    {
        for (int i = 0; i < camsBasic.Length; i++)
        {
            if (i == index)
                camsBasic[i].Priority = 10;
            
            else
                camsBasic[i].Priority = 0;
            
        }
    }

    public void SwitchTutorialCams(int index)
    {
        for (int i = 0; i < camsTutorial.Length; i++)
        {
            if (i == index)
                camsTutorial[i].Priority = 10;
            
            else
                camsTutorial[i].Priority = 0;
            
        }
    }
}
