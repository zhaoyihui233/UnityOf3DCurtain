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

    private static Image item;//图片预制体

    private Image arrowImage;//玩家图片
    // Start is called before the first frame update
    private float rotation = 220.56f;//箭头对齐正北方向所需角度

    private float size = 0f;

    void Start()
    {
        rect = GetComponent<RectTransform>();//获取ShowLittleMap位置
        //virtualCamera = GameObject.FindWithTag("VirtualCamera").transform;//获取玩家位置
        virtualCamera = cinemachineVirtualCamera.transform;//获取虚拟相机的位置？

        item = Resources.Load<Image>("MiniMap/Texture/Image");//动态加载Image类型

        if (virtualCamera != null)
        {
            arrowImage = Instantiate(item);//实例
        }
        arrowImage.rectTransform.sizeDelta = new Vector2(30, 30);//设置箭头大小

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
        arrowImage.rectTransform.SetParent(transform, false);//设置其父物体，父物体为挂载脚本的物体
        arrowImage.rectTransform.anchoredPosition = new Vector2(0f, 0f);//设置playerImage位置

        //小地图是2D，所以旋转在Z轴进行，每次旋转角度为player的旋转（3D上的旋转，基于Y轴旋转）
        arrowImage.rectTransform.eulerAngles = new Vector3(0, 0, -virtualCamera.eulerAngles.y + rotation);
        arrowImage.sprite = Resources.Load<Sprite>("MiniMap/Texture/arrow");//加载图片

    }
    private void OnScreenSizeChanged(float Height)
    {

        float alpha = 0.8f, beta = 0.463f;//alpha，beta为适应屏幕的比例参数，
                                          //前者为本组件尺寸与父组件的比值，后者小地图与屏幕高的比值（都是正方形）
        float height = Height * beta;
        RectTransform parent = transform.parent.GetComponentInParent<RectTransform>();
        RectTransform grandParent = parent.parent.transform.GetComponentInParent<RectTransform>();
        parent.sizeDelta = new Vector2(height, height);
        grandParent.sizeDelta = new Vector2(height, height); ;//父亲与祖父组件的尺寸应该一致
        rect.sizeDelta = new Vector2(height * alpha, height * alpha);
        arrowImage.rectTransform.sizeDelta = new Vector2(Height / 18, Height / 18);//设置箭头大小,箭头图片大小为屏幕高度的1/18
    }

}

