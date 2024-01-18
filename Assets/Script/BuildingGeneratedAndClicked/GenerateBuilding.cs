using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BuildingGenerate : MonoBehaviour
{
    // ����һ�����ڴ洢���������ݵĽṹ��
    private struct CubeData
    {
        public UnityEngine.Vector3 position;  // ����λ��
        public UnityEngine.Vector3 scale;     // ��С
        public int materialIndex; // ������
    };

    public GameObject buildingPrefab;    // ����Ԥ����
    public Material[] buildingMaterials; // �����Ĳ����б�

    public GameObject StadiometryDots, YellowDots;

    private List<GameObject> GeneratedCubeList;
    private List<float[]> VerticesList = new List<float[]>();
    // Awake Start
    private void Awake()
    {
        // ��ȡ CSV �ļ�����������
        List<CubeData> cubeDataList = ReadBuidlingsFromCsv("BuildingCSV/buildingData");

        // ������������������
        GenerateCubes(cubeDataList);
    }

    // ��ָ��csv�ļ�������ݶ�ȡ��CubeData��
    private List<CubeData> ReadBuidlingsFromCsv(string filePath)
    {
        List<CubeData> cubeDataList = new List<CubeData>();

        // ��ȡ CSV �ļ���������
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);
        string[] lists = csvFile.text.Split('\n');

        // ����ÿһ�е�����
        //  ÿһ�����ݵ��ڲ�ͬ���±���
        //  ͬ�в�ͬ�е�����֮���ö��ŷָ�
        for (int i = 0; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // ��� row ����ĳ����Ƿ��㹻��
            if (row.Length < 7)
            {
                continue; // ������Ȳ��㣬˵���ô�û�����ݣ������������ݵĽ���
            }

            // ���ÿ��ֵ�Ƿ�Ϊ�ջ�ֻ�����ո�����ǣ��������������ݵĽ�����
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]) || string.IsNullOrWhiteSpace(row[2]) ||
                string.IsNullOrWhiteSpace(row[3]) || string.IsNullOrWhiteSpace(row[4]) || string.IsNullOrWhiteSpace(row[5]) ||
                string.IsNullOrWhiteSpace(row[6]))
            {
                continue;
            }

            // ����һ�� CubeData �ṹ����󣬲������ݸ�ֵ����
            //  float.Parse�ǽ��ַ���ת����float�ķ���
            CubeData cubeData = new CubeData();
            cubeData.position = new UnityEngine.Vector3(float.Parse(row[0]), float.Parse(row[1]), float.Parse(row[2]));
            cubeData.scale = new UnityEngine.Vector3(float.Parse(row[3]), float.Parse(row[4]), float.Parse(row[5]));
            cubeData.materialIndex = int.Parse(row[6]);

            // �� CubeData ������ӵ��б���
            cubeDataList.Add(cubeData);
        }

        return cubeDataList;
    }

    // ����CubeData�����������������
    private void GenerateCubes(List<CubeData> cubeDataList)
    {
        GeneratedCubeList = new List<GameObject>();

        int i = 1;
        // ���������б�����ÿ�� CubeData �������Ϣ����������
        foreach (CubeData cubeData in cubeDataList)
        {
            // ����һ���µ���������Ϸ����
            GameObject cube = Instantiate(buildingPrefab, cubeData.position, Quaternion.identity);
            cube.name = "part" + i.ToString();

            // ���������������
            cube.transform.localScale = cubeData.scale;

            // ����������Ĳ���
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            renderer.material = buildingMaterials[cubeData.materialIndex - 1];

            // ��� MeshCollider ���
            cube.AddComponent<MeshCollider>().convex = true;
            cube.GetComponent<MeshCollider>().enabled = true; // ���� MeshCollider��ʹ��������Ա����
            cube.GetComponent<MeshCollider>().convex = true;  // ʹ��͹����ײ�����������

            cube.transform.parent = transform;

            ClickBuilding clickBuilding = cube.AddComponent<ClickBuilding>();  // ��� ClickHandler ����������¼�s
            clickBuilding.StadiometryDots = StadiometryDots;
            clickBuilding.YellowDots = YellowDots;

            i++;
            GeneratedCubeList.Add(cube);

            VerticesList.Add(new float[6] {
                cubeData.position.x - cubeData.scale.x / 2,
                cubeData.position.x + cubeData.scale.x / 2,
                cubeData.position.y - cubeData.scale.y / 2,
                cubeData.position.y + cubeData.scale.y / 2,
                cubeData.position.z - cubeData.scale.z / 2,
                cubeData.position.z + cubeData.scale.z / 2
            });//��������ı߽���Ϣ
        }
    }
    public List<GameObject> cubeList
    {
        get
        {
            return GeneratedCubeList;
        }
    }

    // ���� VerticesList ����
    public List<float[]> verticesList
    {
        get
        {
            return VerticesList;
        }
    }
}
