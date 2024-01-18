using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;  // ���浱ǰλ��

        newPosition.x = cinemachineVirtualCamera.transform.position.x;  // �޸� x ����
        newPosition.z = cinemachineVirtualCamera.transform.position.z;
        transform.position = newPosition;
    }
}
