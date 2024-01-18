using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkyController : MonoBehaviour
{
    string path = "Skybox/Materials";
 
    private void SwitchWeather(string idx)
    {
        Material weatherMaterial = null;
        switch (idx)
        {
            case "0":
                weatherMaterial = Resources.Load<Material>(Path.Combine(path, "Skybox_Daytime"));
                break;
            case "1":
                weatherMaterial = Resources.Load<Material>(Path.Combine(path, "Skybox_Sunset"));
                break;
            default:
                Debug.Log("无法找到材质");
                break;

        }
        if (weatherMaterial != null)
        {
            RenderSettings.skybox = weatherMaterial;
        }
    }


}
