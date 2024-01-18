using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class BuildingGenerate : MonoBehaviour
{
    // 定义一个用于存储立方体数据的结构体
    private struct CubeData
    {
        public UnityEngine.Vector3 position;  // 绝对位置
        public UnityEngine.Vector3 scale;     // 大小
        public int materialIndex; // 材质名
    };

    public GameObject buildingPrefab;    // 建筑预制体
    public Material[] buildingMaterials; // 建筑的材质列表

    public GameObject StadiometryDots, YellowDots;

    private List<GameObject> GeneratedCubeList;
    private List<float[]> VerticesList = new List<float[]>();
    // Awake Start
    private void Awake()
    {
        // 读取 CSV 文件并解析数据
        List<CubeData> cubeDataList = ReadBuidlingsFromCsv("BuildingCSV/buildingData");

        // 根据数据生成立方体
        GenerateCubes(cubeDataList);
    }

    // 从指定csv文件里把数据读取到CubeData里
    private List<CubeData> ReadBuidlingsFromCsv(string filePath)
    {
        List<CubeData> cubeDataList = new List<CubeData>();

        // 读取 CSV 文件所有内容
        TextAsset csvFile = Resources.Load<TextAsset>(filePath);
        string[] lists = csvFile.text.Split('\n');

        // 解析每一行的数据
        //  每一行数据的在不同的下标里
        //  同行不同列的数据之间用逗号分隔
        for (int i = 0; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // 检查 row 数组的长度是否足够长
            if (row.Length < 7)
            {
                continue; // 如果长度不足，说明该处没有数据，跳过此行数据的解析
            }

            // 检查每个值是否为空或只包含空格。如果是，则跳过此行数据的解析。
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]) || string.IsNullOrWhiteSpace(row[2]) ||
                string.IsNullOrWhiteSpace(row[3]) || string.IsNullOrWhiteSpace(row[4]) || string.IsNullOrWhiteSpace(row[5]) ||
                string.IsNullOrWhiteSpace(row[6]))
            {
                continue;
            }

            // 创建一个 CubeData 结构体对象，并将数据赋值给它
            //  float.Parse是将字符串转换成float的方法
            CubeData cubeData = new CubeData();
            cubeData.position = new UnityEngine.Vector3(float.Parse(row[0]), float.Parse(row[1]), float.Parse(row[2]));
            cubeData.scale = new UnityEngine.Vector3(float.Parse(row[3]), float.Parse(row[4]), float.Parse(row[5]));
            cubeData.materialIndex = int.Parse(row[6]);

            // 将 CubeData 对象添加到列表中
            cubeDataList.Add(cubeData);
        }

        return cubeDataList;
    }

    // 根据CubeData里的数据生成立方体
    private void GenerateCubes(List<CubeData> cubeDataList)
    {
        GeneratedCubeList = new List<GameObject>();

        int i = 1;
        // 遍历数据列表，根据每个 CubeData 对象的信息生成立方体
        foreach (CubeData cubeData in cubeDataList)
        {
            // 创建一个新的立方体游戏对象
            GameObject cube = Instantiate(buildingPrefab, cubeData.position, Quaternion.identity);
            cube.name = "part" + i.ToString();

            // 设置立方体的缩放
            cube.transform.localScale = cubeData.scale;

            // 设置立方体的材质
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            renderer.material = buildingMaterials[cubeData.materialIndex - 1];

            // 添加 MeshCollider 组件
            cube.AddComponent<MeshCollider>().convex = true;
            cube.GetComponent<MeshCollider>().enabled = true; // 启用 MeshCollider，使立方体可以被点击
            cube.GetComponent<MeshCollider>().convex = true;  // 使用凸包碰撞器，提高性能

            cube.transform.parent = transform;

            ClickBuilding clickBuilding = cube.AddComponent<ClickBuilding>();  // 添加 ClickHandler 组件处理点击事件s
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
            });//各个顶点的边界信息
        }
    }
    public List<GameObject> cubeList
    {
        get
        {
            return GeneratedCubeList;
        }
    }

    // 定义 VerticesList 属性
    public List<float[]> verticesList
    {
        get
        {
            return VerticesList;
        }
    }
}
