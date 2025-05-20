using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TranscriptDialogueLine : EditorWindow
{
    public Line lineRef;
    public Dialogue dialogueRef;
    public List<string> actorNames;

    private TextField lineField;
    private Label startTimeLabel;
    private Label endTimeLabel;
    private Label actorLabel;
    private Label actorName;
    private DropdownField actorDrop;

    private TranscriptWindow twReference;
    private int lineIndex;

    public void PopulateDialogueLine(TranscriptWindow tW, Line line, Dialogue dialogue, List<string> names)
    {

        lineRef = line;
        dialogueRef = dialogue;
        actorNames = names;
        twReference = tW;
        lineIndex = dialogue.lines.IndexOf(line);

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

        // Importa UXML creado manualmente
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SubtitleGenerator/Scripts/Editor/Window/TranscriptDialogLine.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Cada ventana del editor contiene una raiz a su VisualElement
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
        lineField.RegisterValueChangedCallback(evt => { UpdateLineContent(evt.newValue); });
    }

    public void DropDownUpdateSpeaker()
    {
        actorName.text = "> " + actorDrop.value;
        lineRef.actorKey = actorDrop.value;
    }

    private void UpdateLineContent(string newContent)
    {
        lineRef.line = newContent;
        twReference.setLineFromIndex(lineIndex, newContent, lineRef.actorKey);
    }

    public void UpdateSpeakerName(string defaultName, string newName) {
        if (actorName.text == defaultName) {
            string trimedDefaultName = defaultName.Replace("> ", "");

            // Cambiar en la interfaz
            actorName.text = "> " + newName;
            actorDrop.value = newName;
            int index = actorDrop.choices.IndexOf(trimedDefaultName);
            if (index != -1) actorDrop.choices[actorDrop.choices.IndexOf(trimedDefaultName)] = newName;
            actorDrop.MarkDirtyRepaint();
            // En la línea
            lineRef.actorKey = newName;
            twReference.setLineFromIndex(lineIndex, lineRef.line, lineRef.actorKey);
            // Cambiar el actor en el dialogo
            if (dialogueRef.actors.ContainsKey(trimedDefaultName))
            {
                Actor actorAux = dialogueRef.actors[trimedDefaultName];
                actorAux.name = newName;
                dialogueRef.actors.Remove(trimedDefaultName);
                dialogueRef.actors[newName] = actorAux;
            }
        }
    }

    public Line getLine()
    {
        return lineRef;
    }
}

