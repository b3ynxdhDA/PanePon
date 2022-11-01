using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Setting : MonoBehaviour
{
    [SerializeField] GameObject u_ChangeScreenText = null;

    //trueならWindow,falseならFullScreen
    private bool isScreenMode = false;

    //
    const int width = 1920;
    private void Start()
    {
        if (Screen.width < width)
        {
            u_ChangeScreenText.GetComponent<Text>().text = "< Window >";
            isScreenMode = true;
        }
        else
        {
            u_ChangeScreenText.GetComponent<Text>().text = "< Full Screen >";
            isScreenMode = false;
        }
    }

    public void OnScreenChange()
    {
        if (isScreenMode)
        {
            isScreenMode = false;
            Screen.SetResolution(Screen.width, Screen.height, false);
            u_ChangeScreenText.GetComponent<Text>().text = "< Full Screen >";
        }
        else
        {
            isScreenMode = true;
            Screen.SetResolution(Screen.width, Screen.height, true);
            u_ChangeScreenText.GetComponent<Text>().text = "< Window >";
        }
    }

    void Update()
    {
        
    }
}
