using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
/*using System.Runtime.InteropServices;*/


public class CameraSystem : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField]
    private bool isKeyboard = true;
    [SerializeField]
    private bool isMouse = true;
    private float FollowOffsetMax = 300f;
    private float FollowOffsetMin = 5f;
    private float FollowOffsetMaxY = 300f;
    private float FollowOffsetMinY = 5f;

    [SerializeField]
    private Mode mode;
    private enum Mode
    {
        Offset,
        OffsetY
    }


    private bool MouseActive = false;//��¼����Ƿ���
    private Vector2 LastMousePosition;//��¼�ϴ�����λ��
    private Vector3 FollowOffset;
    private bool CtrlActive = false;//��¼ctrl��
    private Vector3 preFollowOffset = new Vector3(0, 0, -180);//��ʼλ�õ����������������λ��


    // Update is called once per frame

    private void Awake()
    {
        FollowOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }

    private void Update()
    {

        if (!CanvasWall.isMoving)
        {
            if (!Input.GetMouseButton(0))
            {
                if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                {
                    CtrlActive = true;
                }
                else
                    CtrlActive = false;
            }
            if (isKeyboard)
            {
                HandleCameraMovement();
                HandleCameraRotate();
            }
            if (isMouse)
            {

                switch (mode)
                {
                    case Mode.Offset:
                        HandleCameraZoom_MoveForward();
                        break;
                    case Mode.OffsetY:
                        HandleCameraZoom_LowerY();
                        break;
                }


                if (!CtrlActive)//����ƶ����
                    HandleCameraMovementMouse();
                else
                    HandleCameraRotate_Mouse();
            }
        }
        else
            FollowOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;

    }

    private void HandleCameraMovement()//WASD�ƶ����
    {

        Vector3 InputDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W)) InputDir.z = 1f;
        if (Input.GetKey(KeyCode.S)) InputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) InputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) InputDir.x = 1f;
        Vector3 MoveDir = transform.forward * InputDir.z + transform.right * InputDir.x;
        float MoveSpeed = 60f;
        transform.position += MoveDir * MoveSpeed * Time.deltaTime;
    }


    /*private void HandleCameraMovementEdgeScroll()//��Ե�ж��ƶ����
    {
        Vector3 InputDir = new Vector3(0, 0, 0);
        int edgeScrollSize = 20;
        
        if (Input.mousePosition.x < edgeScrollSize)InputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize)InputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize)InputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize)InputDir.z = +1f;
        
        Vector3 MoveDir = transform.forward * InputDir.z + transform.right * InputDir.x;
        float MoveSpeed = 60f;
        transform.position += MoveDir * MoveSpeed * Time.deltaTime;
    }
*/
    private void HandleCameraMovementMouse()//�������ƶ�
    {
        Vector3 InputDir = new Vector3(0, 0, 0);
        if (Input.GetMouseButtonDown(0))
        {
            MouseActive = true;
            LastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            MouseActive = false;
        }

        if (MouseActive)
        {
            Vector2 MovePosition = LastMousePosition - (Vector2)Input.mousePosition;
            float MouseSpeed = 2f;
            InputDir.x = MovePosition.x * MouseSpeed;
            InputDir.z = MovePosition.y * MouseSpeed;
            LastMousePosition = Input.mousePosition;
        }
        Vector3 MoveDir = transform.forward * InputDir.z + transform.right * InputDir.x;
        float MoveSpeed = 5f;
        transform.position += MoveDir * MoveSpeed * Time.deltaTime;

    }
    private void HandleCameraRotate()
    {
        //QE��ת���
        float Rotate = 0f;
        if (Input.GetKey(KeyCode.Q))
            Rotate = +1f;
        if (Input.GetKey(KeyCode.E))
            Rotate = -1f;
        float RotateSpeed = 100f;
        transform.eulerAngles += new Vector3(0, Rotate * RotateSpeed * Time.deltaTime, 0);
    }




    private void HandleCameraZoom_MoveForward()
    {
        Vector3 ZoomDir = FollowOffset.normalized;
        float ZoomAmount = 3f;
        if (Input.mouseScrollDelta.y > 0)
        {
            FollowOffset -= ZoomDir * ZoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            FollowOffset += ZoomDir * ZoomAmount;
        }
        if (FollowOffset.magnitude < FollowOffsetMin)
        {
            FollowOffset = ZoomDir * FollowOffsetMin;
        }
        if (FollowOffset.magnitude > FollowOffsetMax)
        {
            FollowOffset = ZoomDir * FollowOffsetMax;
        }
        float ZoomSpeed = 10f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, FollowOffset, Time.deltaTime * ZoomSpeed);
        //ƽ������
    }
    private void HandleCameraZoom_LowerY()
    {

        float ZoomAmount = 3f;
        if (Input.mouseScrollDelta.y > 0)
        {
            FollowOffset.y -= ZoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            FollowOffset.y += ZoomAmount;
        }
        FollowOffset.y = Mathf.Clamp(FollowOffset.y, FollowOffsetMinY, FollowOffsetMaxY);
        float ZoomSpeed = 10f;
        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, FollowOffset, Time.deltaTime * ZoomSpeed);
        //ƽ������
    }

    private void HandleCameraRotate_Mouse()
    {
        Vector3 InputDir = new Vector3(0, 0, 0);
        if (Input.GetMouseButtonDown(0))
        {
            MouseActive = true;
            LastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            MouseActive = false;
        }
        if (MouseActive && CtrlActive)
        {

            Vector2 MovePosition = LastMousePosition - (Vector2)Input.mousePosition;
            float MouseSpeed = 0.1f;
            InputDir = InputDir.normalized;
            InputDir.x = -MovePosition.x * MouseSpeed;
            InputDir.y = -MovePosition.y * MouseSpeed;
            LastMousePosition = Input.mousePosition;
        }
        //if (!Mathf.Approximately(FollowOffset.x, 0)&&!Mathf.Approximately(FollowOffset.z, 0))

        float angle = InputDir.y; // ��ת�Ƕȣ��Զ�Ϊ��λ��
        Vector3 axis = new Vector3(FollowOffset.z, 0, -FollowOffset.x); // ��ת��
        axis = axis.normalized;//��׼��
        axis = axis * Time.deltaTime;
        Quaternion rotation = Quaternion.AngleAxis(angle, axis); // ������תQuaternion

        Vector3 rotatedVector = rotation * FollowOffset; // ����������ת
        /*if (!(Mathf.Abs(rotatedVector.x) < 1f || Mathf.Abs(rotatedVector.z) < 1f))*/
        if (rotatedVector.z < 0f)
        {    
            FollowOffset = rotatedVector;   
           /* if (Math.Abs(rotatedVector.x-0f)<=0.001f && Math.Abs(rotatedVector.z - 0f) <= 0.001f)
            {
                rotatedVector.z -= 1.0f;
                FollowOffset = rotatedVector;
            }
            Debug.Log("rotatedVector" + rotatedVector);
            Debug.Log("transform.rotation.y" + transform.rotation.eulerAngles.y);
            Debug.Log("cinemachineVirtualCamera" + cinemachineVirtualCamera.transform.rotation.eulerAngles.y);*/
            /*if (Math.Abs(transform.rotation.eulerAngles.y - cinemachineVirtualCamera.transform.rotation.eulerAngles.y) > 2.0f)
            {
                Quaternion newRotation = transform.rotation;
                newRotation = Quaternion.Euler(newRotation.eulerAngles.x, cinemachineVirtualCamera.transform.rotation.y, newRotation.eulerAngles.z);
                transform.rotation = newRotation;
            }*/
            
            //cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = FollowOffset;
        }

        float Rotate = InputDir.x;

        transform.eulerAngles += new Vector3(0, Rotate, 0);
    }




    public void SetMode(string index)//�ı����ģʽ
    {
        mode = (Mode)(int.Parse(index));
    }

    public void ResetCamerasystem()
    {
        if (!CanvasWall.isMoving)
        {
            transform.position = new Vector3(60, 0, 0);
            transform.rotation = Quaternion.Euler(0, 180, 0);//rotation��Ϊ0��0��0��ŷ����
            FollowOffset = preFollowOffset;
        }
    }

    public void SetMouse(string value)
    {
        isMouse = bool.Parse(value);
    }
    public void SetKey(string value)
    {
        isKeyboard = bool.Parse(value);
    }
    /*[DllImport("__Internal")]
    private static extern void PostScore(string currentScene);  //��ǰ����*/

    public void SetSize(string type)
    {
        if (!CanvasWall.isMoving)
        {

            Vector3 ZoomDir = FollowOffset.normalized;
            float ZoomAmount = 3f;
            if (type == "1") //1Ϊ�Ŵ���Ҫ�޸����������֮���followset�����ڳ�ʼ���λ������zС����0����
                             //��ʱ�������壬��Ҫ��Сz����Զ������Ҫ����z
            {
                FollowOffset -= ZoomDir * ZoomAmount;
            }
            if (type == "0") //0Ϊ��С
            {
                FollowOffset += ZoomDir * ZoomAmount;
            }
            if (FollowOffset.magnitude < FollowOffsetMin)
            {
                FollowOffset = ZoomDir * FollowOffsetMin;
            }
            if (FollowOffset.magnitude > FollowOffsetMax)
            {
                FollowOffset = ZoomDir * FollowOffsetMax;
            }
            float ZoomSpeed = 10f;
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(
                cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, FollowOffset, Time.deltaTime * ZoomSpeed);
        }
    }
    public void SetRotating(string type)
    {
        if (!CanvasWall.isMoving)
        {
            if (type == "0")
                transform.eulerAngles += new Vector3(0, 90f, 0);//˳ʱ��90��
            if (type == "1")
                transform.eulerAngles += new Vector3(0, -90f, 0);
        }
    }


    public void setFollowOffset(Vector3 s)//�����������ϵͳ��FollowOffset
    {
        FollowOffset = s;
    }
}
