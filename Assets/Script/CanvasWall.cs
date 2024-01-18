
using Accord.Statistics.Moving;
using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CanvasWall : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    public CameraSystem cameraSystem;
    public Image imagePrefab;//ͼƬԤ����
    public BuildingGenerate objects;//cbue��Ľű�����
    //public Transform CameraSystem;//���ϵͳ�����λ������
    private string csvPath = "BuildingCSV/GlobalPicture";//ǽ��ͼƬ���ļ�����
    private List<float[]> verticesList;
    public static bool IsMoving = false;//���ϵͳ�����Ƿ������ƶ���ת
    public static Vector3 clickPosition;
    [DllImport("__Internal")]
    private static extern void Show(string str, string type);

    public static bool isMoving
    {
        get { return IsMoving; }
        set
        {

            if (IsMoving != value)
            {
                IsMoving = value;

                if (IsMoving == false)// ��ֵ�����ı�ʱ�����Զ����¼�
                {
                    Show("0", "2");
                }
                else if (IsMoving == true)
                {
                    Show("1", "2");//��ֵΪ1��ʾ�����ƶ�
                }

            }

        }
    }
    
   

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ClickBuilding()//��ȡ�û��ϴεĵ��λ��
    }

    public void test()
    {
        SetQuick("A");
    }


    private float weightValue(Vector3 val, float[] cube)//�жϸõ���������֮��Ĺ�ϵ
    {
        float result = 0f;
        if (!(val.x > cube[0] && val.x < cube[1] || val.x < cube[0] && val.x > cube[1]))
        {
            float diff1 = Mathf.Abs(val.x - cube[0]);
            float diff2 = Mathf.Abs(val.x - cube[1]);

            result += Mathf.Min(diff1, diff2);
        }
        if (!(val.y > cube[2] && val.y < cube[3] || val.y < cube[2] && val.y > cube[3]))
        {
            float diff1 = Mathf.Abs(val.y - cube[2]);
            float diff2 = Mathf.Abs(val.y - cube[3]);

            result += Mathf.Min(diff1, diff2);
        }
        if (!(val.z > cube[4] && val.z < cube[5] || val.z < cube[4] && val.z > cube[5]))
        {
            float diff1 = Mathf.Abs(val.z - cube[4]);
            float diff2 = Mathf.Abs(val.z - cube[5]);

            result += Mathf.Min(diff1, diff2);
        }
        return result;
    }
    private Vector3 getNormalVector(Vector3 point, GameObject cube)
    {

        Vector3 cubeCenter = cube.transform.position;

        float minDotProduct = float.MaxValue;
        Vector3 faceNormal = Vector3.zero;
        Vector3 cubeScale = new Vector3(cube.transform.localScale.x / 2, cube.transform.localScale.y / 2, cube.transform.localScale.z / 2);
        // ��ȡ������ķ�����
        Vector3[] normalVector = {
            cube.transform.forward, -cube.transform.forward,
            cube.transform.right, -cube.transform.right,
            cube.transform.up, -cube.transform.up
        };

        // ����������ķ�����
        foreach (Vector3 normal in normalVector)
        {
            // ����������������ĵ����������
            Vector3 cubeToCubePoint = point - (cubeCenter + new Vector3(cubeScale.x * normal.x, cubeScale.y * normal.y, cubeScale.z * normal.z));
            // ������
            float dotProduct = Mathf.Abs(Vector3.Dot(cubeToCubePoint, normal));

            // ����������Ͷ�Ӧ�ķ�����
            if (dotProduct < minDotProduct)
            {
                minDotProduct = dotProduct;
                faceNormal = normal;
            }
        }

        return faceNormal;//���ط�����
    }
    private Vector3 getVerticalPosition(Vector3 point, Vector3 normal, GameObject cube)
    {
        point = point + normal;//ponit�ط������������죬��֤��cube�⡣

        // ��������ĵ㲻������������ϣ���ʹ�����߼������������ཻ��
        Ray ray = new Ray(point, -normal);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // ȷ���ཻ��������������
            if (hit.transform == cube.transform)
            {
                return hit.point;
            }
        }
        // ���û���ཻ���Ҹ����ĵ㲻������������ϣ����ؿ�����
        return Vector3.zero;
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        // ��ָ��·������ͼƬ�ļ�
        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        // ����Texture2D�����ֽ������м���ͼƬ����
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        return texture;
    }
    public void SetQuick(string ch)
    {

        if (!string.IsNullOrEmpty(csvPath) && !isMoving)
        {

            string[] csvLines = Resources.Load<TextAsset>(csvPath).text.Trim().Split('\n');//��ȡ�ļ��������з���
            for (int i = 0; i < csvLines.Length; i++)
            {
                int num = 0;
                float result = float.MaxValue;
                string[] data = csvLines[i].Split(',');
                string lastItem = data[data.Length - 1].Trim();//��ȡ���ÿ�����һ��Ԫ�أ�ͼƬ�����֣�


                Debug.Log(lastItem.LastIndexOf(ch));
                if (lastItem.LastIndexOf(ch) == 0)//���ҵ�һ����A��ͷ��ͼƬ������
                {
                    Vector3 position = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));//���������
                    for (int j = 0; j < objects.cubeList.Count; j++)//������ȡcube
                    {
                        float p = weightValue(position, objects.verticesList[j]);
                        if (p < result)
                        {
                            result = p;
                            num = j;
                        }
                    }
                    Vector3 normal = getNormalVector(position, objects.cubeList[num]);//��ȡ���淨����
                    position = objects.cubeList[num].transform.position;//��ȡcube����������������

                    Quaternion targetRotation = Quaternion.LookRotation(-normal, Vector3.up);//Ŀ��Ƕ�

                    StartCoroutine(MoveAndRotate(position, targetRotation));//Э����ת���ƶ�

                    break;//ֱ���˳�
                }
            }
        }
    }
    // Э���ƶ�����ת����
    IEnumerator MoveAndRotate(Vector3 targetPosition, Quaternion targetRotation)//
    {
        isMoving = true;
        float smoothTime = 1f;
        Vector3 velocity = Vector3.zero;
        Vector3 FollowOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        while (Vector3.Distance(cameraSystem.transform.position, targetPosition) > 0.05f ||
            Quaternion.Angle(cameraSystem.transform.rotation, targetRotation) > 0.05f)
        {
            cameraSystem.transform.position = Vector3.SmoothDamp(cameraSystem.transform.position, targetPosition, ref velocity, smoothTime);
            cameraSystem.transform.rotation = Quaternion.Slerp(cameraSystem.transform.rotation, targetRotation, Time.deltaTime * smoothTime * 2);

            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(
             cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, new Vector3(0, 0, FollowOffset.z), Time.deltaTime * 10f);
            yield return null;
        }
        isMoving = false;
    }

    public void setOn()
    {
        isMoving = true;
        if (clickPosition != null )
        {
            int num = 0;
            float result = float.MaxValue;
            Vector3 position = clickPosition;//���Ե�����
            for (int j = 0; j < objects.cubeList.Count; j++)//������ȡcube
            {
                float p = weightValue(position, objects.verticesList[j]);
                if (p < result)
                {
                    result = p;
                    num = j;
                }
            }
            Vector3 normal = getNormalVector(position, objects.cubeList[num]);//��ȡ���淨����
            //position = objects.cubeList[num].transform.position;//��ȡcube����������������

            Quaternion targetRotation = Quaternion.LookRotation(-normal, Vector3.up);//Ŀ��Ƕ�

            //StartCoroutine(MoveAndRotate(position, targetRotation));//Э����ת���ƶ�
            cameraSystem.transform.position = clickPosition;
            cameraSystem.transform.rotation = targetRotation;
            /* cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x = 0;
             cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = 0;
             cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -20f;*/
            cameraSystem.setFollowOffset(new Vector3(0, 0, -20f));
            Debug.Log(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset);
            //���������offset��z��ʾ��ʱ���������ľ���

        }
        isMoving = false;
    }
}
