using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static SubtitleManager;

public class ActorsFoldout : EditorWindow
{
    TextField actorField;
    Label actorName;
    ColorField actorColor;

    TranscriptWindow transcriptWindow;
    string actor;

    public void PopulateActorFoldout(string ac)
    {
        actor = ac;
    }

    [MenuItem("Window/SubtitleGenerator/DEBUG/ActorsFoldout")]
    public static void ShowExample()
    {
        ActorsFoldout wnd = GetWindow<ActorsFoldout>();
        wnd.titleContent = new GUIContent("ActorsFoldout");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML created manually.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SubtitleGenerator/Scripts/Editor/Window/ActorsFoldout.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Each editor window contains a root VisualElement object
        actorName = root.Q<Label>("actorName");
        actorField = root.Q<TextField>("actorField");
        actorColor = root.Q<ColorField>("actorColor");

        SetCallBacks();
        UpdateFields();
    }

    private void UpdateFields()
    {
        if (actor != "")
        {
            actorName.text = "> " + actor;
            actorColor.value = Color.white;
        }
    }

    private void SetCallBacks()
    {
        actorField.RegisterValueChangedCallback(evt =>
        {
            ChangeSpeakers(actorName.text, evt.newValue);
        });
    }

    public void SetWindow(TranscriptWindow tW)
    {
        transcriptWindow = tW;
    }

    private void ChangeSpeakers(string defaultName, string newName)
    {
        actorName.text = "> " + newName;
        foreach (TranscriptDialogueLine td in transcriptWindow.getTranscriptsList())
        {
            td.UpdateSpeakerName(defaultName, newName);
        }
    }
}


