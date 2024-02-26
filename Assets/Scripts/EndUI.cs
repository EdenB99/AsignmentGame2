using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using TreeEditor;
using UnityEngine;

public class EndUI : MonoBehaviour
{
    TextMeshProUGUI[] infoText;
    //[current time : ]
    public string currenttime;
    //[average Speed : ]
    public float averageSpeed = 0;
    //[Over-the-line : ]
    bool isEnd = false;

    private void Awake()
    {
        infoText = GetComponentsInChildren<TextMeshProUGUI>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        isEnd = true;
        setText();
    }
    void setText()
    {
        infoText[1].text = $"[current time : {currenttime}]\r\n\r\n[average Speed : {averageSpeed}]\r\n\r\n[Over-the-line : ]";
    }
}
