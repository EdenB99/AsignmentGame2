using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    public TextMeshProUGUI Speedtext;
    public TextMeshProUGUI TimeText;
    public CarBase carBase;
    public EndUI endUI;
    public float timesec = 0;
    float timemin = 0 ;
    public bool isStart;
    public float speedAdd;
    public float avergeSpeed;
    float kmhSpeed;
    private void Start()
    {
       
    }
    void Update()
    {
        kmhSpeed = carBase.currentSpeed * 1.2f;
        if (isStart)
        {
            timesec += Time.deltaTime;
            if (timesec >= 60f)
            {
                timemin++;
                timesec = 0;
                speedAdd = kmhSpeed;
            }
            
            avergeSpeed = Mathf.FloorToInt(speedAdd/timemin);
            SetText();
        }
        endUI.averageSpeed = avergeSpeed;
        endUI.currenttime = timeText;
        endUI.outlinecount = carBase.outOfLine;
    }
    string timeText;
    private void SetText()
    {
        Speedtext.text = kmhSpeed.ToString("N0") + " Km/h";
        if (timemin >= 10f)
        {
            if (timesec >= 10f)
            {
                TimeText.text = timemin.ToString("N0") + " : " + timesec.ToString("N0");
            } else
            {
                TimeText.text = timemin.ToString("N0") + " : 0" + timesec.ToString("N0");
            }
        } else
        {
            if (timesec >= 10f)
            {
                TimeText.text = "0"+timemin.ToString("N0") + " : " + timesec.ToString("N0");
            } else
            {
                TimeText.text = "0" + timemin.ToString("N0") + " : 0" + timesec.ToString("N0");
            }
        }
        timeText = TimeText.text;
        
    }   

    private void OnEnable()
    {
        isStart = true;
    }
    private void OnDisable()
    {
        isStart = false;
    }
}
