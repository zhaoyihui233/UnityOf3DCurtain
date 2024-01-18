using System;
using UnityEngine;
using System.Runtime.InteropServices;

public class ClickBuilding : MonoBehaviour
{
    private LoadMatchCSV LoadMatchCSVComponent;

    public GameObject StadiometryDots;
    private StadiometryControlling StadiometryControllingComponent;

    public GameObject YellowDots;
    private GenerateLineAndUAV GenerateLineAndUAVComponent;
    public static Vector3 currentPosition;

    [DllImport("__Internal")]
    private static extern void Show(string str, string type);

    private void Start()
    {
        // ��ȡ��LoadMatchCSV���
        Transform parentTransform = transform.parent;
        LoadMatchCSVComponent = parentTransform.GetComponent<LoadMatchCSV>();

        // ��ȡ��StadiometryControlling���
        StadiometryControllingComponent = StadiometryDots.transform.GetComponent<StadiometryControlling>();

        GenerateLineAndUAVComponent = YellowDots.transform.GetComponent<GenerateLineAndUAV>();
    }

    private void OnMouseDown()
    {
        // ��ȡ���λ������Ļ�е�����
        UnityEngine.Vector3 clickPosition = Input.mousePosition;

        // ͨ�����߼�����ж��Ƿ�����������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            // ��ȡ�����������
            GameObject cube = gameObject;
            // ��ȡ�����������
            string cubeName = cube.name;

            Vector3 worldPosition = hit.point;
            CanvasWall.clickPosition = worldPosition;
            // ������λ����������������ĵ��������
            Vector3 relativePosition = worldPosition - cube.transform.position;

            // ��ӡ�����Ϣ
            Debug.Log("(Unity)���λ�ã�" + worldPosition.ToString());
            Debug.Log("(Unity)����������壺" + cubeName);
            Debug.Log("(Unity)������꣺" + relativePosition.ToString());

            float x = worldPosition.x;
            float y = worldPosition.y;
            float z = worldPosition.z;


            if (!StadiometryControllingComponent.IsStadiometry())   // Ѱ��ƥ���ͼƬ
                SearchPicture(worldPosition, relativePosition, cubeName, gameObject);
            else
                StadiometryControllingComponent.GenerateDots(worldPosition);
        }
    }


    private void SearchPicture(Vector3 worldPosition, Vector3 relativePosition, string cubeName, GameObject cube)
    {
        string imagePath = "";
        string strPos = string.Format("{0:0.00},{1:0.00},{2:0.00}", worldPosition.x, worldPosition.y, worldPosition.z);
        // ͨ�� KD ����ȡ������������ĵ�
        if ((cubeName == "part4" || cubeName == "part5" || cubeName == "part9" || cubeName == "part10") && Math.Abs(relativePosition.x + 24.00) < 0.00001)
        {
            imagePath = LoadMatchCSVComponent.SearchImage(worldPosition, 'A');
            GenerateLineAndUAVComponent.generateLineAndUAV(worldPosition, imagePath, 'A');
            if (imagePath != "")
                imagePath = "/DZGCG/Pictures/A/" + imagePath;
        }
        else if (((cubeName == "part3" || cubeName == "part4" || cubeName == "part5" || cubeName == "part10") && Math.Abs(relativePosition.x - 24.00) < 0.00001)
            || ((cubeName == "part8" || cubeName == "part9") && Math.Abs(relativePosition.x - 23.72) < 0.00001))
        {
            imagePath = LoadMatchCSVComponent.SearchImage(worldPosition, 'B');
            GenerateLineAndUAVComponent.generateLineAndUAV(worldPosition, imagePath, 'B');
            if (imagePath != "")
                imagePath = "/DZGCG/Pictures/B/" + imagePath;
        }
        else if ((cubeName == "part10" && Math.Abs(relativePosition.z - 12.10) < 0.0001)
            || (cubeName == "part5" && Math.Abs(relativePosition.z - 6.67) < 0.0001)
            || (cubeName == "part2" && Math.Abs(relativePosition.z - 15.5751) < 0.0001)
            || (cubeName == "part7" && Math.Abs(relativePosition.z - 27.135) < 0.0001))
        {
            imagePath = LoadMatchCSVComponent.SearchImage(worldPosition, 'C');
            GenerateLineAndUAVComponent.generateLineAndUAV(worldPosition, imagePath, 'C');
            if (imagePath != "")
                imagePath = "/DZGCG/Pictures/C/" + imagePath;
        }

        Debug.Log("(Unity)" + strPos + ',' + imagePath + '\r');
        Show(strPos + ',' + imagePath + '\r', "0");
        //YellowDotsControllingComponent.displayDots(worldPosition.x.ToString() + "," + worldPosition.y.ToString() + "," + worldPosition.z.ToString());
        //if (Vector3.Distance(YellowDotsControllingComponent.oldPos, Vector3.zero) > 0.1)
        //    YellowDotsControllingComponent.hideDots(YellowDotsControllingComponent.oldPos.x.ToString() + "," + YellowDotsControllingComponent.oldPos.y.ToString() + "," + YellowDotsControllingComponent.oldPos.z.ToString());
        //YellowDotsControllingComponent.oldPos = worldPosition;
    }
}

