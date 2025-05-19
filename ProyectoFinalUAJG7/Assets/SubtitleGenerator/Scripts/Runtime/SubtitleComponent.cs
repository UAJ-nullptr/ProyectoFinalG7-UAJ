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
    [SerializeField]
    private Color defaultColor = Color.white;
    private int maxWidth = 1250;

    #region Setters
    public void setText(SubtitleManager.SubtitleInfo subInfo)
    {
        transform.position = anchorPosition;
        string hex = ColorUtility.ToHtmlStringRGBA(subInfo.talkerColor);
        string text = "<color=#" + hex + ">";
        text += subInfo.talker;
        text += "</color>";
        text += "-" + subInfo.content;

        textComponent.text = text;

        float textCanvasWidth = textComponent.preferredWidth;

        // Se ajusta el tamaño horizontal del texto a un máximo definido
        if (textCanvasWidth > maxWidth)
        {
            textCanvasWidth = maxWidth;
        }

        // Se ajusta el tamaño al nuevo ancho
        textRect.sizeDelta = new Vector2(textCanvasWidth + 40, textRect.sizeDelta.y);
        // Se fuerza la actualización del canvas
        Canvas.ForceUpdateCanvases();

        // Con la nueva medida horizontal se calcula la nueva altura
        float textCanvasHeight = textComponent.preferredHeight;
        // Se asignan las dimensiones finales del fondo y el texto
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

    void Update()
    {
        
    }
}
