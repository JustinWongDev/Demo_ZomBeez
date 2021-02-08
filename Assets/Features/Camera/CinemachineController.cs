using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private CinemachineVirtualCamera[] cams = null; //0 menu; 1 default game
    [SerializeField] private CinemachineVirtualCamera[] camsTutorial = null; //0 hive; 1 depot; 2 caches; book

    public void SwitchToCam(int index)
    {
        for (int i = 0; i < cams.Length; i++)
        {
            if (i == index)
                cams[i].Priority = 10;
            
            else
                cams[i].Priority = 0;
            
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
