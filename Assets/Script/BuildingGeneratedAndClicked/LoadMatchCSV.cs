using System.Collections.Generic;
using UnityEngine;
using Accord.Collections;    // ���������ռ�
using System.Linq;

public class LoadMatchCSV : MonoBehaviour
{
    public List<ImgRecord> ImgRecordsA, ImgRecordsB, ImgRecordsC; // ͼƬ���ĵ�����
    public KDTree<string> treeA, treeB, treeC;

    // ͼƬɢ�����ģ�͵ľ���
    private float LayerThickness = 1;

    // ƥ��ͼƬ�ľ�����ֵ
    public static float disThreshold = 2;

    // һ��ƥ���������
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

        // ����kd-tree
        treeA = new KDTree<string>(3);
        treeB = new KDTree<string>(3);
        treeC = new KDTree<string>(3);
    }

    private void OnEnable()
    {
        // ��ȡ CSV �ļ���������
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

    // ��CSV�ļ����ȡ����
    private void ReadPointsRecordFromCsv(string filename, char facet)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(filename);
        string[] lists = csvFile.text.Split('\n');

        // ����ÿһ�е�����
        //  ÿһ�����ݵ��ڲ�ͬ���±���
        //  ͬ�в�ͬ�е�����֮���ö��ŷָ�
        for (int i = 1; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // ��� row ����ĳ����Ƿ��㹻��
            if (row.Length < 5)
            {
                continue; // ������Ȳ��㣬˵���ô�û�����ݣ������������ݵĽ���
            }

            // ���ÿ��ֵ�Ƿ�Ϊ�ջ�ֻ�����ո�����ǣ��������������ݵĽ�����
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1])
                || string.IsNullOrWhiteSpace(row[2]) || string.IsNullOrWhiteSpace(row[3])
                || string.IsNullOrWhiteSpace(row[4]))
            {
                continue;
            }

            row[4] = row[4].TrimEnd('\r');

            // �жϵ��Ƿ��ھ��볤����1������,�����򲻲��øõ�
            if (!Islegal(float.Parse(row[0]), float.Parse(row[1]), float.Parse(row[2]), facet))
                continue;

            // ����һ�� PointsRecord ����󣬲������ݸ�ֵ����
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
                if (point.Z >= 68.27f - 4.75f / 2 + 0.2f && point.Z <= 68.27f + 4.75f / 2 - 0.2f) // �ж�ɢ���Ƿ��ڵ�9�����������(0.2����������Χ,ʹ��ɢ�������������)
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

                    // ���⴦�� C ���������
                    if (point.X > 100.32f)
                        point.X = point.X - 2;
                }
                else if (point.Z > 77.31f)
                {
                    point.Z = 77.31f + 13.34f / 2 + LayerThickness;

                    // ���⴦��C���3�������(part5�ϵ�)
                    if (point.X > 76.32f + 24)
                        point.X = point.X - 2;

                    // ���⴦�� C ��İ˸������
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

            // �� PointsRecord ������ӵ��б���
            if (facet == 'A')
                ImgRecordsA.Add(point);
            else if (facet == 'B')
                ImgRecordsB.Add(point);
            else if (facet == 'C')
                ImgRecordsC.Add(point);
        }
    }

    // �ж�ɢ���Ƿ����ģ�ͳ���1��
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

    // �ж�ɢ���Ƿ���ָ��������1������
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

    // �ж�ͼƬ��Ļǽ�Ƿ��ڵ�����
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

    // �ж�ָ��ͼƬ�Ƿ���ImgRecordsA(/B/C)�Ŀǰ�ڽ������˻�λ����ͼƬ�Ķ�Ӧ��ϵʱ��Ҫ�õ���
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

    // ����ƥ��ͼƬ
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
        string imgInfo = $"(Unity)ƥ�䵽��{numNode}��ͼƬ: ", disInfo = $"(Unity)���λ����{numNode}��ͼƬ���ľ��룺";
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
        Debug.Log($"(Unity)�����ͼƬ: {ImgName}");
        Debug.Log($"(Unity)������룺{minDistance}");

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
