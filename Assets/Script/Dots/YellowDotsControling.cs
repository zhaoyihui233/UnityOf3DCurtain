using System.Collections.Generic;
using UnityEngine;

public class YellowDotsControling : MonoBehaviour
{
    public GameObject dotPrefab;
    private List<GameObject> dots;
    private Color oriColor = Color.yellow;
    private Color selColor = Color.red;
    private GenerateLineAndUAV GenerateLineAndUAVComponent;

    void Start()
    {
        GenerateLineAndUAVComponent = transform.GetComponent<GenerateLineAndUAV>();

        dots = new List<GameObject>(); // ʵ���� dots �б�
    }

    //int i = 0;
    //private void Update()
    //{
    //    if (i == 0)
    //    {
    //        displayDots("51.98,13.18,83.34");
    //        displayDots("52.11,14.48,82.39");
    //        displayDots("52.35,17.98,65.98");
    //        highlightDots("52.11,14.48,82.39");

    //        displayDots("100.35,20.38,21.53");
    //        displayDots("100.09,17.08,29.02");
    //        displayDots("100.88,9.38,12.13");
    //        highlightDots("100.88,9.38,12.13");

    //        displayDots("-0.35,-20.38,-21.53");

    //        i = 1;
    //    }
    //}

    public Vector3 oldPos = Vector3.zero;
    public void displayDots(string positionStr)
    {
        // ��������"52.12,13.28,85.74"
        // ת���������
        Vector3 pos = str2Vector3(positionStr);
        // ���ָ������ĵ���ڣ����½���
        foreach (GameObject d in dots)
        {
            float distance = Vector3.Distance(d.transform.position, pos);
            if (distance < 0.0001)
            {
                Debug.Log("(Unity)(displayDots)Ҫ��ʾ�ĵ㣺" + positionStr);

                return;
            }
        }

        // ����Cube
        GameObject dot = Instantiate(dotPrefab, pos, Quaternion.identity);

        // ������ɫ
        Renderer dotRenderer = dot.GetComponent<Renderer>();
        dotRenderer.material.color = oriColor;

        // ���ø��ڵ�
        dot.transform.parent = transform;

        // ������б�
        dots.Add(dot);
    }
    public void hideDots(string positionStr)
    {
        // ת���������
        Vector3 pos = str2Vector3(positionStr);

        // Ѱ��ָ������ĵ㣬������ɾ��
        foreach (GameObject dot in dots)
        {
            //Debug.Log("(Unity)" + pos.ToString());
            float distance = Vector3.Distance(dot.transform.position, pos);
            //Debug.Log($"(Unity)distance: {distance}");
            if (distance < 0.001)
            {
                GenerateLineAndUAVComponent.hideLineAndUAV(dots.IndexOf(dot));
                Destroy(dot);
                dots.Remove(dot); // �����ｫ����б���ɾ��

                Debug.Log($"(Unity)dots.Count: {dots.Count}");
                return;
            }
        }
    }
    public void highlightDots(string positionStr)
    {
        // ת���������
        Vector3 pos = str2Vector3(positionStr);

        // Ѱ��ָ������ĵ�
        foreach (GameObject dot in dots)
        {
            float distance = Vector3.Distance(dot.transform.position, pos);
            if (distance < 0.001)
            {
                Renderer dotRenderer = dot.GetComponent<Renderer>();
                if (dotRenderer.material.color == oriColor)
                {
                    GenerateLineAndUAVComponent.highlightLine(dots.IndexOf(dot));
                    dotRenderer.material.color = selColor;
                }
                else
                {
                    GenerateLineAndUAVComponent.resetLine(dots.IndexOf(dot));
                    dotRenderer.material.color = oriColor;
                }
            }
            else
            {
                GenerateLineAndUAVComponent.resetLine(dots.IndexOf(dot));
                Renderer dotRenderer = dot.GetComponent<Renderer>();
                dotRenderer.material.color = oriColor;
            }
        }
    }

    public Vector3 str2Vector3(string str)
    {
        string[] posArr = str.Split(',');

        float x = float.Parse(posArr[0]);
        float y = float.Parse(posArr[1]);
        float z = float.Parse(posArr[2]);

        return new Vector3(x, y, z);
    }
}
