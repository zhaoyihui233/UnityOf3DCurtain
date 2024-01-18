using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class Loader : MonoBehaviour
{
    public Slider progressBar;      // 进度条控件
    public GameObject building;//利用立方块建立的模型
    public GameObject pictures;//外部图片
    private bool isLoaded = false;
    private bool isRendered = false;
    Renderer[] renderers;
    Renderer[] renderersBuilding;
    
    void Start()
    {
        renderersBuilding = building.GetComponentsInChildren<Renderer>();
        
    }
    public void loadModel()
    {
        if (!isLoaded)
        {
            StartCoroutine(loadAssetBundles());

            isRendered = true;
            renderers = gameObject.GetComponentsInChildren<Renderer>();//物理模型的渲染组件
            disableBuilding();//隐藏相应模型
            isLoaded = true;
        }
        else
        {
            if (isRendered)//此时渲染的是真实模型
            {

                disable();//隐藏真实模型
                enableBuilding();
            }
            else
            {
                disableBuilding();
                enable();//显示真实模型
            }
            isRendered = !isRendered;
        }


    }


    private IEnumerator loadAssetBundles()
    {
        string path1 = Application.streamingAssetsPath + "/blockb";
        string path2 = Application.streamingAssetsPath + "/blocky";

        float progress = 0f;
        progressBar.gameObject.SetActive(true);
        progressBar.value = progress;
        // 异步加载第一个 AssetBundle
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path1))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load file from StreamingAssets: " + www.error);
            }
            else
            {
                // 从获取的数据创建AssetBundle
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if (assetBundle != null)
                {
                    // 从 AssetBundle 加载 GameObject
                    var assetRequest = assetBundle.LoadAssetAsync<GameObject>("BlockB");

                    while (!assetRequest.isDone) // 显示进度
                    {
                        progressBar.value = assetRequest.progress / 2;
                        yield return null;
                    }

                    yield return assetRequest;

                    if (assetRequest.asset != null)
                    {
                        // 实例化 GameObject 并设置父节点
                        GameObject obj = Instantiate(assetRequest.asset as GameObject);
                        obj.transform.SetParent(transform);
                    }
                    else
                    {
                        Debug.LogError("Failed to load asset from AssetBundle: " + path1);
                    }

                    // 卸载 AssetBundle
                    assetBundle.Unload(false);
                }
                else
                {
                    Debug.LogError("Failed to load AssetBundle from StreamingAssets: " + path1);
                }
            }

        }
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path2))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load file from StreamingAssets: " + www.error);
            }
            else
            {
                // 从获取的数据创建AssetBundle
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if (assetBundle != null)
                {
                    // 从 AssetBundle 加载 GameObject
                    var assetRequest = assetBundle.LoadAssetAsync<GameObject>("BlockY");

                    while (!assetRequest.isDone) // 显示进度
                    {
                        progressBar.value = 0.5f + assetRequest.progress / 2;
                        yield return null;
                    }
                    yield return assetRequest;

                    if (assetRequest.asset != null)
                    {
                        // 实例化 GameObject 并设置父节点
                        GameObject obj = Instantiate(assetRequest.asset as GameObject);
                        obj.transform.SetParent(transform);
                    }
                    else
                    {
                        Debug.LogError("Failed to load asset from AssetBundle: " + path2);
                    }
                    // 卸载 AssetBundle
                    assetBundle.Unload(false);
                }
                else
                {
                    Debug.LogError("Failed to load AssetBundle from StreamingAssets: " + path2);
                }
            }

            // 加载完成后设置进度为1
            progressBar.value = 1f;
            progressBar.gameObject.SetActive(false);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            isRendered = true;//加载成功之后该值为真
            renderers = gameObject.GetComponentsInChildren<Renderer>();//物理模型的渲染组件
        }
    }
    private void disable()
    {

        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }
    }
    private void disableBuilding()
    {
        foreach (Renderer renderer in renderersBuilding)
        {
            renderer.enabled = false;
        }
        pictures.SetActive(false);
        

    }
    private void enable()
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = true;
        }

    }
    private void enableBuilding()
    {
        foreach (Renderer renderer in renderersBuilding)
        {
            renderer.enabled = true;
        }
        pictures.SetActive(true);

    }
}
