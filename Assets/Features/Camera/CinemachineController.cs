using System;
using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    #region Static Singleton
    public static CinemachineController Instance { get; private set; }
    private void Awake() => Instance = this;
    #endregion
    
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private CinemachineVirtualCamera[] camsBasic = null;    //0 menu; 1 default game
    [SerializeField] private CinemachineVirtualCamera[] camsTutorial = null; //0 hiveController; 1 depot; 2 caches; book
    [SerializeField] private CinemachineClearShot facilityCamController = null;
    [SerializeField] private Transform defaultFacCamPos = null;
    
    private HumanController human = null;

    public void SetTargetHuman(HumanController human)
    {
        this.human = human;
        facilityCamController.m_LookAt = human.transform;
    }

    public void SetTargetToMiddle()
    {
        facilityCamController.m_LookAt = defaultFacCamPos;
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
