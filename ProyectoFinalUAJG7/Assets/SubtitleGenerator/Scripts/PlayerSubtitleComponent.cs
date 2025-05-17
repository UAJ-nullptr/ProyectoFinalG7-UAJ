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
        if (subtitlesOn) text.text = activateText;
        else text.text = deactivateText;
    }
    
    void Start()
    {
        activateText = "Press E to activate subtitles";
        deactivateText = "Press E to deactivate subtitles";
        subtitlesOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E)) ChangeSubtitles();
    }
}
