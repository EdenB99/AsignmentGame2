using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpeedText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public CarBase carBase;

    void Update()
    {
        float kmhSpeed = carBase.currentSpeed*1.2f;
        text.text = kmhSpeed.ToString("N0") + " Km/h";
    }
}
