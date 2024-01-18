using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LittleMap : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    private RectTransform rect;//

    private Transform virtualCamera;

    private static Image item;//ͼƬԤ����

    private Image arrowImage;//���ͼƬ
    // Start is called before the first frame update
    private float rotation = 220.56f;//��ͷ����������������Ƕ�

    private float size = 0f;

    void Start()
    {
        rect = GetComponent<RectTransform>();//��ȡShowLittleMapλ��
        //virtualCamera = GameObject.FindWithTag("VirtualCamera").transform;//��ȡ���λ��
        virtualCamera = cinemachineVirtualCamera.transform;//��ȡ���������λ�ã�

        item = Resources.Load<Image>("MiniMap/Texture/Image");//��̬����Image����

        if (virtualCamera != null)
        {
            arrowImage = Instantiate(item);//ʵ��
        }
        arrowImage.rectTransform.sizeDelta = new Vector2(30, 30);//���ü�ͷ��С

    }

    // Update is called once per frame
    void Update()
    {
        ShowPlayerImage();
        //if (size != Screen.height)
        //{
        //    size = Screen.height;
        //    OnScreenSizeChanged(size);
        //}
    }


    private void ShowPlayerImage()
    {
        arrowImage.rectTransform.SetParent(transform, false);//�����丸���壬������Ϊ���ؽű�������
        arrowImage.rectTransform.anchoredPosition = new Vector2(0f, 0f);//����playerImageλ��

        //С��ͼ��2D��������ת��Z����У�ÿ����ת�Ƕ�Ϊplayer����ת��3D�ϵ���ת������Y����ת��
        arrowImage.rectTransform.eulerAngles = new Vector3(0, 0, -virtualCamera.eulerAngles.y + rotation);
        arrowImage.sprite = Resources.Load<Sprite>("MiniMap/Texture/arrow");//����ͼƬ

    }
    private void OnScreenSizeChanged(float Height)
    {

        float alpha = 0.8f, beta = 0.463f;//alpha��betaΪ��Ӧ��Ļ�ı���������
                                          //ǰ��Ϊ������ߴ��븸����ı�ֵ������С��ͼ����Ļ�ߵı�ֵ�����������Σ�
        float height = Height * beta;
        RectTransform parent = transform.parent.GetComponentInParent<RectTransform>();
        RectTransform grandParent = parent.parent.transform.GetComponentInParent<RectTransform>();
        parent.sizeDelta = new Vector2(height, height);
        grandParent.sizeDelta = new Vector2(height, height); ;//�������游����ĳߴ�Ӧ��һ��
        rect.sizeDelta = new Vector2(height * alpha, height * alpha);
        arrowImage.rectTransform.sizeDelta = new Vector2(Height / 18, Height / 18);//���ü�ͷ��С,��ͷͼƬ��СΪ��Ļ�߶ȵ�1/18
    }

}

