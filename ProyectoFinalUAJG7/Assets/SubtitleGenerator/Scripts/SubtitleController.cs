using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Button activateAll;
    [SerializeField] private Button multipleSpeakers;
    [SerializeField] private Dropdown dropdown;
    [SerializeField] private Dropdown lenguage;
    [SerializeField] private InputField color;
    [SerializeField] private Button bold;

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

    public void MultipleSpeakers()
    {
        for (int i = 0; i < subtitles.Length; i++)
        {
            subtitles[i].inUse = !subtitles[i].inUse;
            subtitles[i].subtitleObj.SetActive(subtitles[i].inUse);
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
        subtitles[1].subtitleObj.SetActive(false);
        subtitles[2].subtitleObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
