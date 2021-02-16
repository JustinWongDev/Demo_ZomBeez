using System;
using Cinemachine;
using UnityEngine;

public class HumanCam : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    
    private GameObject camPos = null;

    private void Update()
    {
        FollowHuman();
    }

    private void FollowHuman()
    {
        if (FindObjectOfType<HumanController>())
            camPos = FindObjectOfType<HumanController>().ReturnCamPos();

        if (!camPos)
            return;
        
        transform.position = Vector3.Lerp(transform.position, camPos.transform.position, speed);
    }
}
