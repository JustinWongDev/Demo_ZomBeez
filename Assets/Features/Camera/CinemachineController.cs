using System;
using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private CinemachineVirtualCamera[] camsBasic = null;    //0 menu; 1 default game
    [SerializeField] private CinemachineVirtualCamera[] camsTutorial = null; //0 hiveController; 1 depot; 2 caches; book
    [SerializeField] private CinemachineClearShot facilityCamController = null;

    private HumanController human = null;

    public void SetTargetHuman()
    {
        this.human = FindObjectOfType<HumanController>();
        facilityCamController.m_LookAt = human.transform;
    }

    public void SetToFacilityCams()
    {
        facilityCamController.Priority = 20;
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
