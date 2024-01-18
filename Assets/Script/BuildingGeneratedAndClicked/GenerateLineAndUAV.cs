using System.Collections.Generic;
using UnityEngine;

public class GenerateLineAndUAV : MonoBehaviour
{
    public GameObject LinePrefab;
    public GameObject UAVPrefab;
    private List<GameObject> lines;
    private List<GameObject> UAVs;

    private LoadUAVCSV LoadUAVCSVComponent;

    public Material oriMaterial, selMaterial;

    private float oriLineWidth = 1f;
    private float selLineWidth = 6f;

    private void OnEnable()
    {
        lines = new List<GameObject>();
        UAVs = new List<GameObject>();
        LoadUAVCSVComponent = transform.GetComponent<LoadUAVCSV>();
    }

    public void generateLineAndUAV(Vector3 ClickPos, string ImgName, char facet)
    {
        Vector3 UAVPos = LoadUAVCSVComponent.DeriveUAVLocation(ImgName, facet);
        //Debug.Log($"(Unity)图片名：{ImgName}, 面: {facet}");
        //Debug.Log($"(Unity)无人机位置: {UAVPos.ToString()}");
        if (UAVPos == Vector3.zero)
        {
            GameObject line0 = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity);
            GameObject UAV0 = Instantiate(UAVPrefab, UAVPos, Quaternion.identity);

            line0.transform.parent = transform;
            UAV0.transform.parent = transform;

            line0.SetActive(false);
            UAV0.SetActive(false);

            lines.Add(line0);
            UAVs.Add(UAV0);
            return;
        }

        GameObject line = Instantiate(LinePrefab, Vector3.zero, Quaternion.identity);
        GameObject UAV = Instantiate(UAVPrefab, UAVPos, Quaternion.identity);

        line.transform.parent = transform;
        UAV.transform.parent = transform;

        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.SetPosition(0, ClickPos);
        lr.SetPosition(1, UAVPos);
        lr.widthMultiplier = oriLineWidth;

        UAV.transform.LookAt(ClickPos);

        Renderer lrr = line.GetComponent<Renderer>();
        lrr.material = oriMaterial;

        lines.Add(line);
        UAVs.Add(UAV);
    }

    public void hideLineAndUAV(int index)
    {
        //Debug.Log("准备摧毁");
        Destroy(lines[index]);
        lines.RemoveAt(index);
        Destroy(UAVs[index]);
        UAVs.RemoveAt(index);
        //Debug.Log($"lines.Count: {lines.Count}");
        //Debug.Log($"UAVs.Count: {UAVs.Count}");
    }

    public void highlightLine(int index)
    {
        Renderer lrr = lines[index].GetComponent<Renderer>();
        lrr.material = selMaterial;
        LineRenderer lrlr = lines[index].GetComponent<LineRenderer>();
        lrlr.widthMultiplier = selLineWidth;
    }

    public void resetLine(int index)
    {
        Renderer lrr = lines[index].GetComponent<Renderer>();
        lrr.material = oriMaterial;
        LineRenderer lrlr = lines[index].GetComponent<LineRenderer>();
        lrlr.widthMultiplier = oriLineWidth;
    }
}