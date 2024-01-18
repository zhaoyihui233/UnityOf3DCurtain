using System.Collections.Generic;
using UnityEngine;
using Accord.Collections;    // 引入命名空间
using System.Linq;

public class LoadMatchCSV : MonoBehaviour
{
    public List<ImgRecord> ImgRecordsA, ImgRecordsB, ImgRecordsC; // 图片中心的坐标
    public KDTree<string> treeA, treeB, treeC;

    // 图片散点距离模型的距离
    private float LayerThickness = 1;

    // 匹配图片的距离阈值
    public static float disThreshold = 2;

    // 一次匹配结点的数量
    private int numNode = 5;

    public class ImgRecord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string ImageName { get; set; }
        public bool IsCovered { get; set; }
    }

    private void Awake()
    {
        ImgRecordsA = new List<ImgRecord>();
        ImgRecordsB = new List<ImgRecord>();
        ImgRecordsC = new List<ImgRecord>();

        // 构建kd-tree
        treeA = new KDTree<string>(3);
        treeB = new KDTree<string>(3);
        treeC = new KDTree<string>(3);
    }

    private void OnEnable()
    {
        // 读取 CSV 文件所有内容
        ReadPointsRecordFromCsv("BuildingCSV/new_A", 'A');
        ReadPointsRecordFromCsv("BuildingCSV/new_B", 'B');
        ReadPointsRecordFromCsv("BuildingCSV/new_C", 'C');

        foreach (ImgRecord point in ImgRecordsA)
        {
            treeA.Add(new double[] { point.X, point.Y, point.Z }, point.ImageName);
        }
        foreach (ImgRecord point in ImgRecordsB)
        {
            treeB.Add(new double[] { point.X, point.Y, point.Z }, point.ImageName);
        }
        foreach (ImgRecord point in ImgRecordsC)
        {
            treeC.Add(new double[] { point.X, point.Y, point.Z }, point.ImageName);
        }
    }

    // 从CSV文件里获取数据
    private void ReadPointsRecordFromCsv(string filename, char facet)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(filename);
        string[] lists = csvFile.text.Split('\n');

        // 解析每一行的数据
        //  每一行数据的在不同的下标里
        //  同行不同列的数据之间用逗号分隔
        for (int i = 1; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // 检查 row 数组的长度是否足够长
            if (row.Length < 5)
            {
                continue; // 如果长度不足，说明该处没有数据，跳过此行数据的解析
            }

            // 检查每个值是否为空或只包含空格。如果是，则跳过此行数据的解析。
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1])
                || string.IsNullOrWhiteSpace(row[2]) || string.IsNullOrWhiteSpace(row[3])
                || string.IsNullOrWhiteSpace(row[4]))
            {
                continue;
            }

            row[4] = row[4].TrimEnd('\r');

            // 判断点是否在距离长方体1米以内,不是则不采用该点
            if (!Islegal(float.Parse(row[0]), float.Parse(row[1]), float.Parse(row[2]), facet))
                continue;

            // 创建一个 PointsRecord 类对象，并将数据赋值给它
            ImgRecord point = new ImgRecord
            {
                X = float.Parse(row[0]),
                Y = float.Parse(row[1]),
                Z = float.Parse(row[2]),
                ImageName = row[3],
                IsCovered = (row[4] == "0")
            };

            if (facet == 'A')
            {
                if (point.Z >= 68.27f - 4.75f / 2 + 0.2f && point.Z <= 68.27f + 4.75f / 2 - 0.2f) // 判断散点是否在第9个长方体表面(0.2用于限缩范围,使得散点得缩进更合理)
                    point.X = 76.04f - 47.44f / 2 - LayerThickness;
                else
                    point.X = 76.32f - 24 - LayerThickness;
            }
            else if (facet == 'B')
            {
                point.X = 76.32f + 24 + LayerThickness;

            }
            else
            {
                if (point.Z > 96)
                {
                    point.Z = 96.08f + 24.2f / 2 + LayerThickness;

                    // 特殊处理 C 面的三个点
                    if (point.X > 100.32f)
                        point.X = point.X - 2;
                }
                else if (point.Z > 77.31f)
                {
                    point.Z = 77.31f + 13.34f / 2 + LayerThickness;

                    // 特殊处理C面的3个特殊点(part5上的)
                    if (point.X > 76.32f + 24)
                        point.X = point.X - 2;

                    // 特殊处理 C 面的八个特殊点
                    if (point.Y < 6.78f + 13.55f / 2 + 0.3f)
                        point.Y = 6.78f + 13.55f / 2 + 0.6f;
                }
                else if (point.X >= 50.13f - 4.39f / 2 + 0.2f && point.X <= 50.13f + 4.39f / 2 - 0.2f)
                {
                    point.Z = 27.73f + 54.27f / 2 + LayerThickness;

                }
                else
                {
                    point.Z = 39.28f + 31.15f / 2 + LayerThickness;

                }

                if (point.X < 0)
                    point.X = point.X + 1.2f;
            }

            // 将 PointsRecord 对象添加到列表中
            if (facet == 'A')
                ImgRecordsA.Add(point);
            else if (facet == 'B')
                ImgRecordsB.Add(point);
            else if (facet == 'C')
                ImgRecordsC.Add(point);
        }
    }

    // 判断散点是否距离模型超过1米
    private bool Islegal(float x, float y, float z, char facet)
    {
        if (facet == 'A')
        {
            if (IsInCube(x, y, z, "part4") || IsInCube(x, y, z, "part5") || IsInCube(x, y, z, "part9") || IsInCube(x, y, z, "part10"))
                return true;
        }
        else if (facet == 'B')
        {
            if (IsInCube(x, y, z, "part3") || IsInCube(x, y, z, "part4") || IsInCube(x, y, z, "part5")
             || IsInCube(x, y, z, "part8") || IsInCube(x, y, z, "part9") || IsInCube(x, y, z, "part10"))
                return true;
        }
        else if (facet == 'C')
        {
            if (IsInCube(x, y, z, "part2") || IsInCube(x, y, z, "part5") || IsInCube(x, y, z, "part7") || IsInCube(x, y, z, "part10"))
                return true;
        }

        return false;
    }

    // 判断散点是否在指定长方体1米以内
    private bool IsInCube(float x, float y, float z, string cubename)
    {
        Transform cube = transform.Find(cubename);
        if (
        x >= cube.position.x - cube.localScale.x / 2 - 1 && x <= cube.position.x + cube.localScale.x / 2 + 1 &&
        y >= cube.position.y - cube.localScale.y / 2 - 1 && y <= cube.position.y + cube.localScale.y / 2 + 1 &&
        z >= cube.position.z - cube.localScale.z / 2 - 1 && z <= cube.position.z + cube.localScale.z / 2 + 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 判断图片中幕墙是否遮挡严重
    public bool IsCovered(string imgName, char facet)
    {
        ImgRecord result;
        //Debug.Log($"(Unity)imgName: {imgName}, facet: {facet}");
        if (facet == 'A')
            result = ImgRecordsA.FirstOrDefault(record => record.ImageName == imgName);
        else if (facet == 'B')
            result = ImgRecordsB.FirstOrDefault(record => record.ImageName == imgName);
        else
            result = ImgRecordsC.FirstOrDefault(record => record.ImageName == imgName);

        if (result == null)
            return true;
        return result.IsCovered;
    }

    // 判断指定图片是否在ImgRecordsA(/B/C)里（目前在建立无人机位置与图片的对应关系时需要用到）
    public bool IsInImgRecord(string imgName, char facet)
    {
        switch (facet)
        {
            case 'A':
                if (ImgRecordsA.Any(record => record.ImageName == imgName))
                    return true;
                break;
            case 'B':
                if (ImgRecordsB.Any(record => record.ImageName == imgName))
                    return true;
                break;
            case 'C':
                if (ImgRecordsC.Any(record => record.ImageName == imgName))
                    return true;
                break;
        }
        return false;
    }

    // 搜索匹配图片
    public string SearchImage(Vector3 ClickPos, char facet)
    {
        KDTree<string> tree;
        if (facet == 'A')
            tree = treeA;
        else if (facet == 'B')
            tree = treeB;
        else
            tree = treeC;

        var NearPoints = tree.Nearest(new double[] { ClickPos.x, ClickPos.y, ClickPos.z }, numNode);
        float[] distance = new float[numNode];
        string imgInfo = $"(Unity)匹配到的{numNode}张图片: ", disInfo = $"(Unity)点击位置与{numNode}张图片中心距离：";
        int minIndex = 0;
        float minDistance = 999999;
        string ImgName = "";
        for (int i = 0; i < numNode; i++)
        {
            Vector3 point = new Vector3((float)NearPoints[i].Node.Position[0], (float)NearPoints[i].Node.Position[1], (float)NearPoints[i].Node.Position[2]);
            distance[i] = Vector3.Distance(ClickPos, point);

            if (distance[i] < minDistance)
            {
                minDistance = distance[i];
                minIndex = i;
                ImgName = NearPoints[i].Node.Value;
            }

            imgInfo = imgInfo + NearPoints[i].Node.Value + ",";
            disInfo = disInfo + distance[i].ToString() + ",";
        }

        Debug.Log(imgInfo);
        Debug.Log(disInfo);
        Debug.Log($"(Unity)最近的图片: {ImgName}");
        Debug.Log($"(Unity)最近距离：{minDistance}");

        if (facet == 'A' && minDistance <= disThreshold)
        {
            if (!IsCovered(ImgName, 'A'))
                return ImgName;
        }
        else if (facet == 'B' && minDistance <= disThreshold)
        {
            if (!IsCovered(ImgName, 'B'))
                return ImgName;
        }
        else if (facet == 'C' && minDistance <= disThreshold)
        {
            if (!IsCovered(ImgName, 'C'))
                return ImgName;
        }
        return "";
    }
}
