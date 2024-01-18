using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class GenerateTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tipTextPrefab;
    public string tip;
    private RectTransform tipTarget;
    private GameObject newText;

    private UnityEngine.UI.Image img;
    private TMP_Text text;

    private void Awake()
    {
        tipTarget = GetComponent<RectTransform>();
    }
    private void Start()
    {
        newText = Instantiate(tipTextPrefab, tipTarget);
        newText.name = "TipText";
        Vector3 pos = transform.position;
        newText.transform.position = new Vector2(pos.x, pos.y) + new Vector2(0, 20);
        newText.transform.Find("Text").GetComponent<TMP_Text>().text = tip;

        //img = GetComponent<UnityEngine.UI.Image>();
        //text = newText.transform.Find("Text").GetComponent<TMP_Text>();
        //img.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        //Debug.Log(img.transform.position);
        //Debug.Log(text.transform.position);
        //Debug.Log(img.transform.localScale);
        //Debug.Log(text.rectTransform.rect.height);

        newText.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        newText.SetActive(true);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        newText.SetActive(false);
    }

    public void Hide()
    {
        newText.SetActive(false);
    }
}