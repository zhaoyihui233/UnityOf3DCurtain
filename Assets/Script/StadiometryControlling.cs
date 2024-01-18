using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class StadiometryControlling : MonoBehaviour
{

    public GameObject dotPrefab;    // 点的预制体
    private GameObject[] dots;      // 两个点组成的数组
    public Material dotsMaterial;   // 点的材质
    private bool isStadiometry;
    private int NumDots;
    private float[] DifDistance;

    [DllImport("__Internal")]
    private static extern void Show(string str, string type);

    void Awake()
    {
        dots = new GameObject[2];
        DifDistance = new float[3];
    }

    void Start()
    {
        NumDots = 0;
        isStadiometry = false;

        dots[0] = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity);
        dots[0].transform.parent = transform;
        Renderer dotRenderer = dots[0].GetComponent<Renderer>();
        dotRenderer.material = dotsMaterial;
        dots[0].SetActive(false);

        dots[1] = Instantiate(dotPrefab, Vector3.zero, Quaternion.identity);
        dots[1].transform.parent = transform;
        dotRenderer = dots[1].GetComponent<Renderer>();
        dotRenderer.material = dotsMaterial;
        dots[1].SetActive(false);
    }

    public void StartAndFinishStadiometry(string txt)
    {
        if (txt == "true")
            isStadiometry = true;
        else
        {
            isStadiometry = false;
            NumDots = 0;
            dots[0].SetActive(false);
            dots[1].SetActive(false);
        }
    }

    public void GenerateDots(Vector3 pos)
    {
        Debug.Log("(Unity)这是测距功能");
        if (NumDots == 0)
        {
            dots[0].transform.position = pos;
            dots[0].SetActive(true);

            Show("1\r", "1");

            NumDots++;
        }
        else if (NumDots == 1)
        {
            dots[1].transform.position = pos;
            dots[1].SetActive(true);

            DifDistance[0] = (float)Math.Round(Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(dots[0].transform.position.x, dots[0].transform.position.z)), 2);
            DifDistance[1] = (float)Math.Round(Mathf.Abs(pos.y - dots[0].transform.position.y), 2);
            DifDistance[2] = (float)Math.Round(Vector3.Distance(pos, dots[0].transform.position), 2);
            Debug.Log("(Unity)2," + DifDistance[0].ToString() + "," + DifDistance[1].ToString() + "," + DifDistance[2].ToString() + "\r");

            Show("2," + DifDistance[0].ToString() + "m," + DifDistance[1].ToString() + "m," + DifDistance[2].ToString() + "m\r", "1");

            NumDots++;
        }
        else
        {
            dots[0].transform.position = pos;
            dots[1].SetActive(false);

            Show("1\r", "1");

            NumDots = 1;
        }
    }

    public bool IsStadiometry()
    {
        return isStadiometry;
    }
}
