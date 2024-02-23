using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CarBase carBase;

    public float timeText;
    public bool isStart;
    public float speedAdd;
    public float avergeSpeed;
    void Update()
    {
        float kmhSpeed = carBase.currentSpeed * 1.2f;
        text.text = kmhSpeed.ToString("N0") + " Km/h";
        if (isStart )
        {
            timeText += Time.deltaTime;
            speedAdd += kmhSpeed/100;
            avergeSpeed = Mathf.FloorToInt(speedAdd/timeText);
        }
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
