using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//public struct SubtitleInfo
//{
//    public string talker;
//    public string content;
//    public int startTime;
//    public int endTime;
//}

public class SubtitleComponent : MonoBehaviour
{
    private TextMeshProUGUI textComponent;
    private Image backgroundImage;

    [SerializeField]
    private TMP_FontAsset fontAsset;
    [SerializeField]
    private int size = 36;
    [SerializeField]
    private Color color = Color.black;
    [SerializeField]
    private bool background = false;
    [SerializeField] [Range(0, 1)]
    private float backgroundOpacity = 0.5f;
    [SerializeField]
    private bool bold = false;
    [SerializeField]
    private bool multipleSpeakers = false;

    public void setText(SubtitleManager.SubtitleInfo subInfo)
    {
        textComponent.text = multipleSpeakers ? "-" + subInfo.content : subInfo.content;
    }

    public void setFont(TMP_FontAsset f)
    {
        textComponent.font = f;
    }

    public void setSize(int s)
    {
        textComponent.fontSize = s;
    }

    public bool isBold()
    {
        return bold;
    }

    public void setBold(bool b)
    {
        if (b) textComponent.fontStyle = FontStyles.Bold;
        else textComponent.fontStyle = FontStyles.Normal;
        bold = b;
    }

    public void setColor(Color c)
    {
        textComponent.color = color;
    }

    public void setBackground(bool bc)
    {
        background = !background;
        transform.GetChild(0).gameObject.SetActive(background);
    }

    public void setBackgroundOpacity(float op)
    {
        if (background) backgroundImage.color = new Color(0, 0, 0, backgroundOpacity);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Text
        textComponent = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        setFont(fontAsset);
        setColor(color);
        setSize(size);
        setBold(bold);

        // Image
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        transform.GetChild(0).gameObject.SetActive(background);
        setBackgroundOpacity(backgroundOpacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
