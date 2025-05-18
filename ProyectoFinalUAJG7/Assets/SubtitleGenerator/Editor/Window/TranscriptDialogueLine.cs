using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TranscriptDialogueLine : EditorWindow
{
    public Line lineRef;
    public Dialogue dialogueRef;

    TextField lineField;
    Label startTimeLabel;
    Label endTimeLabel;


    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    public void PopulateDialogueLine(Line line, Dialogue dialogue)
    {
        lineRef = line;
        dialogueRef = dialogue;
    }

    [MenuItem("Window/SubtitleGenerator/DEBUG/TranscriptDialogueLine")]
    public static void ShowExample()
    {
        TranscriptDialogueLine wnd = GetWindow<TranscriptDialogueLine>();
        wnd.titleContent = new GUIContent("TranscriptDialogueLine");        
    }

    public void OnValidate()
    {
        lineRef.line = lineField.value;
        Debug.Log(lineRef.line);
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;

        // Import UXML created manually.
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SubtitleGenerator/Editor/Window/TranscriptDialogLine.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // Each editor window contains a root VisualElement object
        lineField = root.Q<TextField>("lineField");
        startTimeLabel = root.Q<Label>("startTime");
        endTimeLabel = root.Q<Label>("endTime");

        lineRef = new Line();

        lineRef.line = "esto es una unidade de prueba";
        lineRef.endTime = 3;
        lineRef.startTime = 0;

        UpdateFields();
    }

    public void UpdateFields()
    {
        if (lineRef.line != "")
        {
            lineField.value = lineRef.line;
            startTimeLabel.text = lineRef.startTime.ToString();
            endTimeLabel.text = lineRef.endTime.ToString();
        }
    }
}
