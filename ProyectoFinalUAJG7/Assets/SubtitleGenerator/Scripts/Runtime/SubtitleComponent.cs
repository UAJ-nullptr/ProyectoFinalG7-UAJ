using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleComponent : MonoBehaviour
{
    // Components
    private TextMeshProUGUI textComponent;
    private RectTransform textRect;
    private Image backgroundImage;
    private RectTransform backgroundRect;

    // Propiedades del texto
    private Vector3 anchorPosition;
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

    private int maxWidth = 1250;

    #region Setters
    public void setText(SubtitleManager.SubtitleInfo subInfo)
    {
        transform.position = anchorPosition;
        textComponent.text = multipleSpeakers ? "-" + subInfo.content : subInfo.content;

        float textCanvasWidth = textComponent.preferredWidth;

        if (textCanvasWidth > maxWidth)
        {
            textCanvasWidth = maxWidth;
        }

        textRect.sizeDelta = new Vector2(textCanvasWidth + 40, textRect.sizeDelta.y);

        Canvas.ForceUpdateCanvases();

        float textCanvasHeight = textComponent.preferredHeight;

        textRect.sizeDelta = new Vector2(textCanvasWidth + 40, textCanvasHeight + 10);
        backgroundRect.sizeDelta = new Vector2(textCanvasWidth + 40, textCanvasHeight + 10);
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
    void Awake()
    {
        // Text
        textComponent = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        textRect = textComponent.GetComponent<RectTransform>();
        setFont(fontAsset);
        setColor(color);
        setSize(size);
        setBold(bold);
        setItalic(italic);

        // Image
        backgroundImage = transform.GetChild(0).GetComponent<Image>();
        backgroundRect = backgroundImage.GetComponent<RectTransform>();
        setBackground(background);
        setBackgroundOpacity(backgroundOpacity);

        anchorPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
