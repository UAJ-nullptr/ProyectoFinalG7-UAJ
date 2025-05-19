using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TranscriptDialogueLine : EditorWindow
{
    public Line lineRef;
    public Dialogue dialogueRef;
    public List<string> actorNames;

    TextField lineField;
    Label startTimeLabel;
    Label endTimeLabel;
    Label actorLabel;
    Label actorName;
    DropdownField actorDrop;

    public void PopulateDialogueLine(Line line, Dialogue dialogue, List<string> names)
    {
        lineRef = line;
        dialogueRef = dialogue;
        actorNames = names;

    }

    [MenuItem("Window/SubtitleGenerator/DEBUG/TranscriptDialogueLine")]
    public static void ShowExample()
    {
        TranscriptDialogueLine wnd = GetWindow<TranscriptDialogueLine>();
        wnd.titleContent = new GUIContent("TranscriptDialogueLine");        
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML created manually.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SubtitleGenerator/Scripts/Editor/Window/TranscriptDialogLine.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Each editor window contains a root VisualElement object
        lineField = root.Q<TextField>("lineField");
        startTimeLabel = root.Q<Label>("startTime");
        endTimeLabel = root.Q<Label>("endTime");

        actorName = root.Q<Label>("lineID");
        actorDrop = root.Q<DropdownField>("actor");
        lineField = root.Q<TextField>("lineField");
        startTimeLabel = root.Q<Label>("startTime");
        endTimeLabel = root.Q<Label>("endTime");

        SetCallBacks();
        UpdateFields();
    }

    private void UpdateFields()
    {
        if (lineRef.line != "")
        {
            lineField.value = lineRef.line;
            startTimeLabel.text = lineRef.startTime.ToString();
            endTimeLabel.text = lineRef.endTime.ToString();

            actorName.text = "> " + lineRef.actorKey;
            actorDrop.choices = actorNames;
            actorDrop.value = lineRef.actorKey;
            lineField.value = lineRef.line;
            startTimeLabel.text = "Start: " + (lineRef.startTime / 1000f).ToString("R");
            endTimeLabel.text = " - End: " + (lineRef.endTime / 1000f).ToString("R");
        }
    }

    private void SetCallBacks()
    {
        actorDrop.RegisterValueChangedCallback(evt => { DropDownUpdateSpeaker(); });
    }

    public void DropDownUpdateSpeaker()
    {
        actorName.text = "> " + actorDrop.value;
    }

    public void UpdateSpeakerName(string defaultName, string newName) {
        int index = actorDrop.choices.IndexOf(defaultName.Replace("> ", ""));
        if (index != -1)
        {
            actorDrop.choices[index] = newName;
        }
        if (actorName.text == defaultName) {
            actorName.text = "> " + newName;
            actorDrop.value = newName;
        }
    }
}

