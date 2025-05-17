using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct usingSubtitles
{
    public GameObject subtitleObj;
    public bool inUse;
}

public class SubtitleController : MonoBehaviour
{
    [SerializeField] private GameObject subtitlesSpeaker;
    [SerializeField] private GameObject subtitlesSpeakerOne;
    [SerializeField] private GameObject subtitlesSpeakerTwo;
    public usingSubtitles[] subtitles = new usingSubtitles[3];

    private bool optionsActivated;
    [SerializeField] private GameObject optionsContainer;
    [SerializeField] private Button activateAll;
    [SerializeField] private Button multipleSpeakers;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Dropdown lenguage;
    [SerializeField] private TMP_InputField color;
    [SerializeField] private Button bold;
    [SerializeField] private Button italic;
    [SerializeField] private Button background;

    [SerializeField] private List<TMP_FontAsset> fonts;

    public void Activate()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            if (subtitles[i].inUse)
            {
                bool isActive = subtitles[i].subtitleObj.activeSelf;
                subtitles[i].subtitleObj.SetActive(!isActive);
            }
        }
        if (optionsContainer != null)
        {
            optionsActivated = !optionsActivated;
            optionsContainer.SetActive(optionsActivated);
        }
    }

    public void Bold()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            if (subtitles[i].inUse)
            {
                bool isBold = subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().isBold();
                subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().setBold(!isBold);
            }
        }
    }

    public void Italic()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            if (subtitles[i].inUse)
            {
                bool isItalic = subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().isItalic();
                subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().setItalic(!isItalic);
            }
        }
    }

    public void Color()
    {
        string[] colorValue = color.text.Split(',');
        if (colorValue.Length == 4)
        {
            float r = float.Parse(colorValue[0], CultureInfo.InvariantCulture.NumberFormat);
            float g = float.Parse(colorValue[1], CultureInfo.InvariantCulture.NumberFormat);
            float b = float.Parse(colorValue[2], CultureInfo.InvariantCulture.NumberFormat);
            float a = float.Parse(colorValue[3], CultureInfo.InvariantCulture.NumberFormat);

            for (int i = 0; i < subtitles.Length; i++)
            {
                if (subtitles[i].inUse)
                    subtitles[i].subtitleObj.GetComponent<SubtitleComponent>()
                        .setColor(new Color(r, g, b, a));
            }
        }
        else
        {
            Debug.LogWarning("Invalid or incomplete format. Use: R, G, B, A [0 - 1]");
        }
    }

    public void Background()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            if(subtitles[i].inUse)
            {
                bool hasBg = subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().hasBackground();
                subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().setBackground(!hasBg);
            }
        }
    }

    public void MultipleSpeakers()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].inUse = !subtitles[i].inUse;
            subtitles[i].subtitleObj.SetActive(subtitles[i].inUse);
        }
    }

    public void OnFontChanged(int value)
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().setFont(fonts[value]);
        }
    }

    public void OnStyleChanged(int value)
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].subtitleObj.GetComponent<SubtitleComponent>().setBackgroundOpacity(1 - 0.5f * value);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        subtitles[0].subtitleObj = subtitlesSpeaker;
        subtitles[1].subtitleObj = subtitlesSpeakerOne;
        subtitles[2].subtitleObj = subtitlesSpeakerTwo;
        subtitles[0].inUse = true;
        subtitles[1].inUse = false;
        subtitles[2].inUse = false;
        subtitles[0].subtitleObj.SetActive(false);
        subtitles[1].subtitleObj.SetActive(false);
        subtitles[2].subtitleObj.SetActive(false);

        if (optionsContainer != null) optionsContainer.SetActive(false);
        optionsActivated = false;

        subtitles[0].subtitleObj.GetComponent<SubtitleComponent>().setColor(new Color(1, 1, 1, 1));
        OnStyleChanged(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
