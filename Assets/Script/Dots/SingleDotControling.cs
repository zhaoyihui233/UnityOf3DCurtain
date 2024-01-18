using System.Collections.Generic;
using UnityEngine;

public class SingleDotControling : MonoBehaviour
{
    public GameObject dotPrefab;
    private List<GameObject> dots;

    void Start()
    {
        dots = new List<GameObject>(); // 实例化 dots 列表
    }

    public void setDot(string positionStr)
    {
        if (positionStr == "")
        {
            foreach (GameObject d in dots)
                Destroy(d);
            dots.Clear();
            return;
        }

        string[] posArr = positionStr.Split(',');

        float x = float.Parse(posArr[0]);
        float y = float.Parse(posArr[1]);
        float z = float.Parse(posArr[2]);

        Vector3 pos = new Vector3(x, y, z);

        GameObject dot = Instantiate(dotPrefab, pos, Quaternion.identity);
        Renderer dotRenderer = dot.GetComponent<Renderer>();
        dotRenderer.material.color = Color.red;
        dot.transform.parent = transform;
        dots.Add(dot);
    }
}
