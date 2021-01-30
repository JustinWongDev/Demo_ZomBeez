using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] private CinemachineBrain camBrain;
    [SerializeField] private CinemachineVirtualCamera[] cams; //0 menu; 1 default game

    public void SwitchToCam(int index)
    {
        for (int i = 0; i < cams.Length; i++)
        {
            if (i == index)
            {
                //cams[i].enabled = true;
                cams[i].Priority = 10;
            }
            else
            {
                //cams[i].enabled = false;
                cams[i].Priority = 0;
            }
        }
    }
}
