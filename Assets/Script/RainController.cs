using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.RainMaker;
using UnityEngine.UI;

public class RainController : MonoBehaviour
{

    public RainScript rainScript;
    // Start is called before the first frame update
    private bool isRain = true;
    public Image image;
    // Update is called once per frame
    void Update()
    {
        
    }
    public void setRain()
    {
        if (rainScript != null)
        {
            // 获取子物体组件的实例

            if (isRain)
            {
                image.enabled =true;
                rainScript.RainIntensity = 0.0f;
            }
            else
            {
                image.enabled =false;
                rainScript.RainIntensity= 1.0f;
            }
            isRain=!isRain;
        }
    }
}
