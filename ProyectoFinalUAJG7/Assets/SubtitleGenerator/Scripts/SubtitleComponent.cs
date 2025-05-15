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
    // Components
    private TextMeshProUGUI textComponent;
    private Image backgroundImage;

    // Propiedades del texto
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
    private bool italic = false;
    [SerializeField]
    private bool multipleSpeakers = false;

    #region Setters
    public void setText(SubtitleManager.SubtitleInfo subInfo)
    {
        textComponent.text = multipleSpeakers ? "-" + subInfo.content : subInfo.content;
    }

    public void setFont(TMP_FontAsset f)
    {
        textComponent.font = f;
        fontAsset = f;
    }

    public void setSize(int s)
    {
        textComponent.fontSize = s;
        size = s;
    }

    public void setBold(bool b)
    {
        textComponent.fontStyle = FontStyles.Normal;
        if (b) textComponent.fontStyle |= FontStyles.Bold;
        if (italic) textComponent.fontStyle |= FontStyles.Italic;
        bold = b;
    }

    public void setItalic(bool i)
    {
        textComponent.fontStyle = FontStyles.Normal;
        if (i) textComponent.fontStyle |= FontStyles.Italic;
        if (bold) textComponent.fontStyle |= FontStyles.Bold;
        italic = i;
    }

    public void setColor(Color c)
    {
        textComponent.color = c;
        color = c;
    }

    public void setBackground(bool bc)
    {
        transform.GetChild(0).gameObject.SetActive(bc);
        background = bc;
    }

    public void setBackgroundOpacity(float op)
    {
        if (background) backgroundImage.color = new Color(0, 0, 0, op);
        backgroundOpacity = op;
    }
    #endregion

    #region Getters
    public bool isBold() { return bold; }

    public bool isItalic() { return italic; }

    public bool hasBackground() { return background; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Text
        textComponent = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        setFont(fontAsset);
        setColor(color);
        setSize(size);
        setBold(bold);
        setItalic(italic);

        // Image
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        setBackground(background);
        setBackgroundOpacity(backgroundOpacity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
