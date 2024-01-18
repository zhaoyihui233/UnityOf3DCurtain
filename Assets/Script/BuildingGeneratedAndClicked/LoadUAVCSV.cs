using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LoadUAVCSV : MonoBehaviour
{
    public List<UAVRecord> UAVRecordsA, UAVRecordsB, UAVRecordsC; // ͼƬ���ĵ�����
    public GameObject Building;
    private LoadMatchCSV LoadMatchCSVComponent;

    // ƥ��ͼƬ�ľ�����ֵ
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

        //Debug.Log($"A��������{UAVRecordsA.Count},B��������{UAVRecordsB.Count},C��������{UAVRecordsC.Count}");
    }

    // ��CSV�ļ����ȡ����
    private void ReadUAVRecordFromCsv(string filename, char facet)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(filename);
        string[] lists = csvFile.text.Split('\n');
        //Debug.Log(lists.Length);
        // ����ÿһ�е�����
        //  ÿһ�����ݵ��ڲ�ͬ���±���
        //  ͬ�в�ͬ�е�����֮���ö��ŷָ�
        for (int i = 0; i < lists.Length; i++)
        {
            string[] row = lists[i].Split(',');
            // ��� row ����ĳ����Ƿ��㹻��
            if (row.Length < 4)
            {
                continue; // ������Ȳ��㣬˵���ô�û�����ݣ������������ݵĽ���
            }

            // ���ÿ��ֵ�Ƿ�Ϊ�ջ�ֻ�����ո�����ǣ��������������ݵĽ�����
            if (string.IsNullOrWhiteSpace(row[0]) || string.IsNullOrWhiteSpace(row[1]) || string.IsNullOrWhiteSpace(row[2]) || string.IsNullOrWhiteSpace(row[3]))
            {
                continue;
            }

            row[3] = row[3].TrimEnd('\r');

            // ȥ�����ڵ���ͼƬ��Ӧ�����˻���¼
            // ȥ���뽨��̫Զ��ͼƬ��Ӧ�����˻���¼
            if (!LoadMatchCSVComponent.IsInImgRecord(row[3], facet) || LoadMatchCSVComponent.IsCovered(row[3], facet))
            {
                continue;
            }
            // ����һ�� UAVRecord ����󣬲������ݸ�ֵ����
            UAVRecord uav = new UAVRecord
            {
                X = float.Parse(row[0]),
                Y = float.Parse(row[1]),
                Z = float.Parse(row[2]),
                ImageName = row[3]
            };

            // �� UAVRecord ������ӵ��б���
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
