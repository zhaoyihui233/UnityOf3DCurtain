
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
    public Image imagePrefab;//图片预制体
    public BuildingGenerate objects;//cbue块的脚本引用
    //public Transform CameraSystem;//相机系统物体的位置引用
    private string csvPath = "BuildingCSV/GlobalPicture";//墙壁图片的文件名曾
    private List<float[]> verticesList;
    public static bool IsMoving = false;//相机系统物体是否正在移动旋转
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

                if (IsMoving == false)// 在值发生改变时触发自定义事件
                {
                    Show("0", "2");
                }
                else if (IsMoving == true)
                {
                    Show("1", "2");//传值为1表示正在移动
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
        //ClickBuilding()//获取用户上次的点击位置
    }

    public void test()
    {
        SetQuick("A");
    }


    private float weightValue(Vector3 val, float[] cube)//判断该点与立方块之间的关系
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
        // 获取六个面的法向量
        Vector3[] normalVector = {
            cube.transform.forward, -cube.transform.forward,
            cube.transform.right, -cube.transform.right,
            cube.transform.up, -cube.transform.up
        };

        // 遍历六个面的法向量
        foreach (Vector3 normal in normalVector)
        {
            // 计算点与立方体中心点的连线向量
            Vector3 cubeToCubePoint = point - (cubeCenter + new Vector3(cubeScale.x * normal.x, cubeScale.y * normal.y, cubeScale.z * normal.z));
            // 计算点积
            float dotProduct = Mathf.Abs(Vector3.Dot(cubeToCubePoint, normal));

            // 更新最大点积和对应的法向量
            if (dotProduct < minDotProduct)
            {
                minDotProduct = dotProduct;
                faceNormal = normal;
            }
        }

        return faceNormal;//返回法向量
    }
    private Vector3 getVerticalPosition(Vector3 point, Vector3 normal, GameObject cube)
    {
        point = point + normal;//ponit沿法向量向外延伸，保证在cube外。

        // 如果给定的点不在立方体的面上，则使用射线检测与立方体的相交点
        Ray ray = new Ray(point, -normal);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 确保相交的物体是立方体
            if (hit.transform == cube.transform)
            {
                return hit.point;
            }
        }
        // 如果没有相交点且给定的点不在立方体的面上，返回空向量
        return Vector3.zero;
    }

    private Texture2D LoadTextureFromFile(string filePath)
    {
        // 从指定路径加载图片文件
        byte[] bytes = System.IO.File.ReadAllBytes(filePath);

        // 创建Texture2D并从字节数组中加载图片数据
        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);

        return texture;
    }
    public void SetQuick(string ch)
    {

        if (!string.IsNullOrEmpty(csvPath) && !isMoving)
        {

            string[] csvLines = Resources.Load<TextAsset>(csvPath).text.Trim().Split('\n');//获取文件，并按行分离
            for (int i = 0; i < csvLines.Length; i++)
            {
                int num = 0;
                float result = float.MaxValue;
                string[] data = csvLines[i].Split(',');
                string lastItem = data[data.Length - 1].Trim();//获取最后每行最后一个元素（图片的名字）


                Debug.Log(lastItem.LastIndexOf(ch));
                if (lastItem.LastIndexOf(ch) == 0)//查找第一个以A打头的图片的坐标
                {
                    Vector3 position = new Vector3(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));//该面的坐标
                    for (int j = 0; j < objects.cubeList.Count; j++)//遍历获取cube
                    {
                        float p = weightValue(position, objects.verticesList[j]);
                        if (p < result)
                        {
                            result = p;
                            num = j;
                        }
                    }
                    Vector3 normal = getNormalVector(position, objects.cubeList[num]);//获取该面法向量
                    position = objects.cubeList[num].transform.position;//获取cube块立方体中心坐标

                    Quaternion targetRotation = Quaternion.LookRotation(-normal, Vector3.up);//目标角度

                    StartCoroutine(MoveAndRotate(position, targetRotation));//协程旋转与移动

                    break;//直接退出
                }
            }
        }
    }
    // 协程移动和旋转物体
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
            Vector3 position = clickPosition;//正对的坐标
            for (int j = 0; j < objects.cubeList.Count; j++)//遍历获取cube
            {
                float p = weightValue(position, objects.verticesList[j]);
                if (p < result)
                {
                    result = p;
                    num = j;
                }
            }
            Vector3 normal = getNormalVector(position, objects.cubeList[num]);//获取该面法向量
            //position = objects.cubeList[num].transform.position;//获取cube块立方体中心坐标

            Quaternion targetRotation = Quaternion.LookRotation(-normal, Vector3.up);//目标角度

            //StartCoroutine(MoveAndRotate(position, targetRotation));//协程旋转与移动
            cameraSystem.transform.position = clickPosition;
            cameraSystem.transform.rotation = targetRotation;
            /* cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.x = 0;
             cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y = 0;
             cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.z = -20f;*/
            cameraSystem.setFollowOffset(new Vector3(0, 0, -20f));
            Debug.Log(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset);
            //重置相机的offset，z表示此时相机与物体的距离

        }
        isMoving = false;
    }
}
