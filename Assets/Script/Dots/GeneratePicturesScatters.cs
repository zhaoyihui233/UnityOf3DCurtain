using System.Collections.Generic;
using UnityEngine;

public class GeneratePicturesScatters : MonoBehaviour
{
    public GameObject dotsPrefab;   // 点的预制体
    private List<GameObject> dots;  // 点的列表
    public Material dotsMaterial;   // 点的材质
    private LoadMatchCSV LoadMatchCSVComponent;


    private void Awake()
    {
        dots = new List<GameObject>();
    }
    void Start()
    {
        LoadMatchCSVComponent = GameObject.Find("Building").GetComponent<LoadMatchCSV>();

        foreach (var point in LoadMatchCSVComponent.ImgRecordsA)
            if (!LoadMatchCSVComponent.IsCovered(point.ImageName, 'A'))
                GenerateDot(point.X, point.Y, point.Z, 'A');
        foreach (var point in LoadMatchCSVComponent.ImgRecordsB)
            if (!LoadMatchCSVComponent.IsCovered(point.ImageName, 'B'))
                GenerateDot(point.X, point.Y, point.Z, 'B');
        foreach (var point in LoadMatchCSVComponent.ImgRecordsC)
            if (!LoadMatchCSVComponent.IsCovered(point.ImageName, 'C'))
                GenerateDot(point.X, point.Y, point.Z, 'C');
    }
    private void GenerateDot(float x, float y, float z, char facet)
    {
        Vector3 pos = new Vector3(x, y, z);
        GameObject dot = Instantiate(dotsPrefab, pos, Quaternion.identity);

        dot.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

        MeshRenderer renderer = dot.GetComponent<MeshRenderer>();
        renderer.material = dotsMaterial;

        dot.transform.parent = transform;

        dot.name = "dot" + (dots.Count + 1).ToString();

        dots.Add(dot);
    }
}
