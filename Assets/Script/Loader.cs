using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class Loader : MonoBehaviour
{
    public Slider progressBar;      // �������ؼ�
    public GameObject building;//���������齨����ģ��
    public GameObject pictures;//�ⲿͼƬ
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
            renderers = gameObject.GetComponentsInChildren<Renderer>();//����ģ�͵���Ⱦ���
            disableBuilding();//������Ӧģ��
            isLoaded = true;
        }
        else
        {
            if (isRendered)//��ʱ��Ⱦ������ʵģ��
            {

                disable();//������ʵģ��
                enableBuilding();
            }
            else
            {
                disableBuilding();
                enable();//��ʾ��ʵģ��
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
        // �첽���ص�һ�� AssetBundle
        using (UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(path1))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load file from StreamingAssets: " + www.error);
            }
            else
            {
                // �ӻ�ȡ�����ݴ���AssetBundle
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if (assetBundle != null)
                {
                    // �� AssetBundle ���� GameObject
                    var assetRequest = assetBundle.LoadAssetAsync<GameObject>("BlockB");

                    while (!assetRequest.isDone) // ��ʾ����
                    {
                        progressBar.value = assetRequest.progress / 2;
                        yield return null;
                    }

                    yield return assetRequest;

                    if (assetRequest.asset != null)
                    {
                        // ʵ���� GameObject �����ø��ڵ�
                        GameObject obj = Instantiate(assetRequest.asset as GameObject);
                        obj.transform.SetParent(transform);
                    }
                    else
                    {
                        Debug.LogError("Failed to load asset from AssetBundle: " + path1);
                    }

                    // ж�� AssetBundle
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
                // �ӻ�ȡ�����ݴ���AssetBundle
                AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(www);

                if (assetBundle != null)
                {
                    // �� AssetBundle ���� GameObject
                    var assetRequest = assetBundle.LoadAssetAsync<GameObject>("BlockY");

                    while (!assetRequest.isDone) // ��ʾ����
                    {
                        progressBar.value = 0.5f + assetRequest.progress / 2;
                        yield return null;
                    }
                    yield return assetRequest;

                    if (assetRequest.asset != null)
                    {
                        // ʵ���� GameObject �����ø��ڵ�
                        GameObject obj = Instantiate(assetRequest.asset as GameObject);
                        obj.transform.SetParent(transform);
                    }
                    else
                    {
                        Debug.LogError("Failed to load asset from AssetBundle: " + path2);
                    }
                    // ж�� AssetBundle
                    assetBundle.Unload(false);
                }
                else
                {
                    Debug.LogError("Failed to load AssetBundle from StreamingAssets: " + path2);
                }
            }

            // ������ɺ����ý���Ϊ1
            progressBar.value = 1f;
            progressBar.gameObject.SetActive(false);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            isRendered = true;//���سɹ�֮���ֵΪ��
            renderers = gameObject.GetComponentsInChildren<Renderer>();//����ģ�͵���Ⱦ���
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
