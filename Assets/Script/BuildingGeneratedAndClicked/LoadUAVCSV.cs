using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadUAVCSV : MonoBehaviour
{
    public List<UAVRecord> UAVRecordsA, UAVRecordsB, UAVRecordsC; // 图片中心的坐标
    public GameObject Building;
    private LoadMatchCSV LoadMatchCSVComponent;

    // 匹配图片的距离阈值
    public static float disThreshold = 2;

    public class UAVRecord
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public string ImageName { get; set; }
    }

    private void Awake()
    {
        UAVRecordsA = new List<UAVRecord>();
        UAVRecordsB = new List<UAVRecord>();
        UAVRecordsC = new List<UAVRecord>();
    }

    private void Start()
    {
        LoadMatchCSVComponent = Building.GetComponent<LoadMatchCSV>();

        ReadUAVRecordFromCsv("BuildingCSV/A_drone", 'A');
        ReadUAVRecordFromCsv("BuildingCSV/B_drone", 'B');
        ReadUAVRecordFromCsv("BuildingCSV/C_drone", 'C');

        //Debug.Log($"A的数量：{UAVRecordsA.Count},B的数量：{UAVRecordsB.Count},C的数量：{UAVRecordsC.Count}");
    }

    // 从CSV文件里获取数据
    private void ReadUAVRecordFromCsv(string filename, char facet)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(filename);
        string[] lists = csvFile.text.Split('\n');
        //Debug.Log(lists.Length);
        // 解析每一行的数据
        //  每一行数据的在不同的下标里
        //  同行不同列的数据之间用逗号分隔
        for (int i = 0; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // 检查 row 数组的长度是否足够长
            if (row.Length < 4)
            {
                continue; // 如果长度不足，说明该处没有数据，跳过此行数据的解析
            }

            // 检查每个值是否为空或只包含空格。如果是，则跳过此行数据的解析。
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]) || string.IsNullOrWhiteSpace(row[2]) || string.IsNullOrWhiteSpace(row[3]))
            {
                continue;
            }

            row[3] = row[3].TrimEnd('\r');

            // 去除被遮挡的图片对应的无人机记录
            // 去除离建筑太远的图片对应的无人机记录
            if (!LoadMatchCSVComponent.IsInImgRecord(row[3], facet) || LoadMatchCSVComponent.IsCovered(row[3], facet))
            {
                continue;
            }
            // 创建一个 UAVRecord 类对象，并将数据赋值给它
            UAVRecord uav = new UAVRecord
            {
                X = float.Parse(row[0]),
                Y = float.Parse(row[1]),
                Z = float.Parse(row[2]),
                ImageName = row[3]
            };

            // 将 UAVRecord 对象添加到列表中
            if (facet == 'A')
                UAVRecordsA.Add(uav);
            else if (facet == 'B')
                UAVRecordsB.Add(uav);
            else if (facet == 'C')
                UAVRecordsC.Add(uav);
        }
    }

    // 
    public Vector3 DeriveUAVLocation(string ImgName, char facet)
    {
        if (facet == 'A')
        {
            UAVRecord re = UAVRecordsA.FirstOrDefault(record => record.ImageName == ImgName);
            if (re != null)
            {
                //Debug.Log($"{re.X}, {re.Y}, {re.Z}");
                return new Vector3(re.X, re.Y, re.Z);
            }
        }
        else if (facet == 'B')
        {
            UAVRecord re = UAVRecordsB.FirstOrDefault(record => record.ImageName == ImgName);
            if (re != null)
            {
                //Debug.Log($"{re.X}, {re.Y}, {re.Z}");
                return new Vector3(re.X, re.Y, re.Z);
            }
        }
        else
        {
            UAVRecord re = UAVRecordsC.FirstOrDefault(record => record.ImageName == ImgName);
            if (re != null)
            {
                //Debug.Log($"{re.X}, {re.Y}, {re.Z}");
                return new Vector3(re.X, re.Y, re.Z);
            }
        }

        return Vector3.zero;
    }
}
