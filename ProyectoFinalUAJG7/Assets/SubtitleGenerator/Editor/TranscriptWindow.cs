using Codice.CM.Client.Differences.Graphic;
using Codice.CM.Common.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static SubtitleManager;



public class TranscriptWindow : EditorWindow
{
    // Elementos de la UI de la ventana en orden de aparicion
    private UnityEditor.UIElements.ObjectField audioFileInput;
    private UnityEngine.UIElements.Button processButton;
    private UnityEditor.UIElements.ObjectField fileInfoInput;
    private Foldout actorsFoldout;
    private UnityEngine.UIElements.Button saveButton;
    private UnityEngine.UIElements.Button exportButton;
    private UnityEngine.UIElements.Button deleteButton;
    private TextField transcriptText;
    private ScrollView scrollView;

    // Audio a procesar
    private AudioClip audioToTranscript;
    private VideoClip videoToTranscript;
    private DialogueManager dialogueManager;
    private Dialogue currentDiag;

    // Añadir al menú contextual y abrir ventana
    // Se hace en "Tools" porque Unity obliga a que sea en esa pestaña por consistencia
    // en la AssetStore
    [MenuItem("Tools/Transcript Audio Tool")]
    public static void OpenEditorWindow()
    {
        TranscriptWindow window = GetWindow<TranscriptWindow>();
        window.titleContent = new GUIContent("Transcript Audio Tool");
        window.maxSize = new Vector2(900, 700);
        window.minSize = window.maxSize;
    }

    // Crear la ventana a partir del UXMl y obtener los elementos
    public void CreateGUI()
    {
        // Crear la GUI
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/SubtitleGenerator/Editor/Window/SubEditorWindow.uxml");
        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);

        // Guardar los elementos
        audioFileInput = root.Q<UnityEditor.UIElements.ObjectField>("audioField");
        processButton = root.Q<UnityEngine.UIElements.Button>("process");
        fileInfoInput = root.Q<UnityEditor.UIElements.ObjectField>("fileInfo");
        actorsFoldout = root.Q<Foldout>("actors");
        saveButton = root.Q<UnityEngine.UIElements.Button>("saveButton");
        exportButton = root.Q<UnityEngine.UIElements.Button>("exportButton");
        deleteButton = root.Q<UnityEngine.UIElements.Button>("deleteButton");
        transcriptText = root.Q<TextField>("transcript");
        scrollView = root.Q<ScrollView>("transcriptElements");

        // Asignar callbacks
        audioFileInput.RegisterValueChangedCallback(AudioSelected);

        //processButton.clicked += ProcessAudio;
        processButton.clicked += ExposeTranscriptElements;

        saveButton.clicked += SaveTranscript;
        exportButton.clicked += ExportTranscript;
        deleteButton.clicked += DeleteOptions;

        dialogueManager = new DialogueManager();

        UnityEngine.Debug.Log("GUI created.");
    }

    // Metodo para cuando se añade el audio
    private void AudioSelected(ChangeEvent<UnityEngine.Object> evt)
    {
        var value = evt.newValue;
        if (value is AudioClip)
        {
            audioToTranscript = (AudioClip) evt.newValue;
            videoToTranscript = null;
            UnityEngine.Debug.Log("\"" + audioToTranscript.name + "\" setted correctly.");
        }
        else if (value is VideoClip)
        {
            videoToTranscript = (VideoClip) evt.newValue;
            audioToTranscript = null;
            UnityEngine.Debug.Log("\"" + videoToTranscript.name + "\" setted correctly.");
        }
        else
        {
            UnityEngine.Debug.LogWarning("Only AudioClips or VideoClips are accepted");
            audioToTranscript = null;
            videoToTranscript = null;
        }
    }

    // Metodo que llama a Whisper y compañia para entonces mostrarlo en el TextField
    private void ProcessAudio()
    {
        UnityEngine.Debug.Log("Process");
        if (audioToTranscript != null || videoToTranscript != null) {
            UnityEngine.Debug.Log("Processing...");

            // Llamar al metodo de Pyhton
            string venvPath = Path.GetFullPath("myENV"); // Carpeta del entorno virtual
            string pythonExe = Path.Combine(venvPath, "Scripts", "python.exe"); // Python del entorno virtual
            string scriptDir = Path.GetFullPath("./Assets/SubtitleGenerator/Python");   // Carpeta donde está el script de Python
            string scriptName = "PyannoteWhisper.py";
            string inputPath = Path.GetFullPath(AssetDatabase.GetAssetPath(
                audioToTranscript != null ? audioToTranscript : videoToTranscript));

            if (!File.Exists(pythonExe))
            {
                UnityEngine.Debug.LogError("No se encontró el ejecutable de Python en el entorno virtual.");
                return;
            }

            if (!File.Exists(Path.Combine(scriptDir, scriptName)))
            {
                UnityEngine.Debug.LogError("No se encontró el script de Python.");
                return;
            }

            //// 1. Crear entorno virtual
            //RunCommand($"{pythonExe} -m venv {venvPath}", scriptDir, true);

            //// 2. Activar el entorno virtual
            //string activateCmd = Path.Combine(venvPath, "Scripts", "activate");
            //RunCommand($"{activateCmd}", scriptDir, true);

            string pythonCmd = $"{scriptName}"; /*{arguments}*/

            // 3. Correr el archivo de python
            RunCommand(pythonExe, $"\"{scriptName}\" \"{inputPath}\"", scriptDir);

            // Podemos dejar definida la carpeta donde se va a encontrar el SRT hardcodeado
            // O podemos intentar sacar el output de python pero creo que eso te saca toda la consola y son muchas cosas

            // Método que expondrá en la ventana los dialogos
            ExposeTranscriptElements();

            UnityEngine.Debug.Log("Processed");
        }
    }

    private void RunCommand(string executer, string command, string workingDirectory, bool useShell = false)
    {
        using (Process process = new Process())
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = executer;
            process.StartInfo.Arguments = $"{command}";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = false;
            
            process.Start();

            if (!useShell)
            {
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                UnityEngine.Debug.Log("Salida:");
                UnityEngine.Debug.Log(output);
                UnityEngine.Debug.LogError("Error:");
                UnityEngine.Debug.LogError(error);
            }

            process.WaitForExit();
        }
    }


    private void ExposeTranscriptElements()
    {
        UnityEngine.Debug.Log("Mostrando texto obtenido");
        string srtPath = "./Assets/SubtitleGenerator/Tests/prueba4.txt"; // -> esto debería pasar como parámetro
        currentDiag = (Dialogue)dialogueManager.ReadTextSRT(srtPath);

        actorsFoldout.Clear();
        List<string> actorsNamesList = new List<string>();
        foreach (var actor in currentDiag.actors)
        {
            TextField actorField = new TextField(actor.Key);
            actorField.RegisterValueChangedCallback(evt =>
            {
                changeSpeakers(actorField, actorField.label, evt.newValue);
            });

            actorsFoldout.Add(actorField);
            actorsNamesList.Add(actor.Key);
        }

        scrollView.Clear();
        VisualTreeAsset dialogLineAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/SubtitleGenerator/Editor/Window/TranscriptDialogLine.uxml");

        foreach (var line in currentDiag.lines)
        {
            // TO-DO: Guardar esto en una lista de clases contenedores donde estén las referencias
            // a los distintos elementos para su posterior modificación 

            TranscriptDialogueLine newDiag = CreateInstance<TranscriptDialogueLine>();
            newDiag.PopulateDialogueLine(line, currentDiag, actorsNamesList);
            newDiag.CreateGUI();
            UnityEngine.Debug.Log(newDiag.rootVisualElement);
            scrollView.contentContainer.Add(newDiag.rootVisualElement);
            //VisualElement newDiag = dialogLineAsset.CloneTree();

            //Label actorName = newDiag.Q<Label>("lineID");
            //DropdownField actorDrop = newDiag.Q<DropdownField>("actor");
            //TextField lineField = newDiag.Q<TextField>("lineField");
            //Label startTimeLabel = newDiag.Q<Label>("startTime");
            //Label endTimeLabel = newDiag.Q<Label>("endTime");

            //actorName.text = "> " + line.actorKey;
            //actorDrop.choices = actorsNamesList;
            //actorDrop.value = line.actorKey;
            //lineField.value = line.line;
            //startTimeLabel.text = "Start: " + (line.startTime / 1000f).ToString("R");
            //endTimeLabel.text = " - End: " + (line.endTime / 1000f).ToString("R");

            //scrollView.Add(newDiag);
        }


    }

    // Guardar la transcripcion
    private void SaveTranscript()
    {
        UnityEngine.Debug.Log("Save");
        if (transcriptText.value != null)
        {
            // Guardar en un archivo?
        }
    }

    // Exportar la transcripcion
    private void ExportTranscript()
    {
        UnityEngine.Debug.Log("Export");
        if (transcriptText.value != null)
        {
            // Guardar en un archivo... otra vez?
        }
    }

    // Setea la interfaz y su info a los valores iniciales
    private void DeleteOptions()
    {
        UnityEngine.Debug.Log("Delete");
        transcriptText.value = null;
        audioToTranscript = null;
    }

    private void changeSpeakers(TextField tf, string defaultName, string newName) { 
        tf.label = newName;
        // TO-DO: Modificar de la template el label y el dropdown, y actualizar en la estructura la info
        var list = scrollView.Query<VisualElement>(className: nameof(TranscriptDialogueLine)).ToList();
        var item = list.ElementAt(INDEX);
    }
}
