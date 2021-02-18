using System;
using Cinemachine;
using UnityEngine;
using TMPro;
using static System.DateTime;

public class FacCamCanvasController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI t_camName = null;
    [SerializeField] private TextMeshProUGUI t_date = null;
    [SerializeField] private TextMeshProUGUI t_time = null;
    [SerializeField] private CinemachineClearShot facCamController = null;

    private void Update()
    {
        SetCamName();
        TickDateTime();
    }

    private void TickDateTime()
    {
        t_time.text = DateTime.Now.TimeOfDay.Hours.ToString() + ":" + DateTime.Now.TimeOfDay.Minutes.ToString() + ":" + DateTime.Now.TimeOfDay.Seconds.ToString("00") + ":" + DateTime.Now.TimeOfDay.Milliseconds.ToString("000");
        t_date.text = DateTime.Today.Day.ToString() + " / " + DateTime.Today.Month.ToString() + " / " + DateTime.Today.Year.ToString();
    }

    private void SetCamName()
    {
        t_camName.text = facCamController.LiveChild.Name.ToString();
    }
}
