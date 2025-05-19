using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSubtitleComponent : MonoBehaviour
{
    [SerializeField] private SubtitleController subtitleController;
    [SerializeField] private TextMeshProUGUI text;

    private string activateText;
    private string deactivateText;
    private bool subtitlesOn;

    void ChangeSubtitles()
    {
        subtitleController.Activate();
        subtitlesOn = !subtitlesOn;
        if (subtitlesOn) text.text = deactivateText;
        else text.text = activateText;
    }
    
    void Start()
    {
        activateText = "Press Q to activate subtitles";
        deactivateText = "Press Q to deactivate subtitles";
        subtitlesOn = false;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q)) ChangeSubtitles();
    }
}
