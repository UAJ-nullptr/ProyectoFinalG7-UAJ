using Codice.CM.Client.Differences.Graphic;
using Codice.CM.Common.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;



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
    private DialogueManager dialogueManager;

    // Añadir al menú contextual y abrir ventana
    // Se hace en "Tools" porque Unity obliga a que sea en esa pestaña por consistencia
    // en la AssetStore
    [MenuItem("Tools/Transcript Audio Tool")]
    public static void OpenEditorWindow()
    {
        TranscriptWindow window = GetWindow<TranscriptWindow>();
        window.titleContent = new GUIContent("Transcript Audio Tool");
        window.maxSize = new Vector2(500, 400);
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
        audioFileInput.objectType = typeof(AudioClip);
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
        processButton.clicked += ProcessAudio;
        saveButton.clicked += SaveTranscript;
        exportButton.clicked += ExportTranscript;
        deleteButton.clicked += DeleteOptions;

        UnityEngine.Debug.Log("GUI created.");
    }

    // Metodo para cuando se añade el audio
    private void AudioSelected(ChangeEvent<UnityEngine.Object> evt)
    {
        UnityEngine.Debug.Log("Audio");
        string path = AssetDatabase.GetAssetPath((AudioClip)evt.newValue);
        if (!path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            UnityEngine.Debug.LogWarning("Only .wav files are allowed to be transcribed.");
            audioToTranscript = null;
        }
        else
        {
            audioToTranscript = (AudioClip)evt.newValue;
            UnityEngine.Debug.Log("\"" + audioToTranscript.name + "\" setted correctly.");
        }
    }

    // Metodo que llama a Wisper y compañia para entonces mostrarlo en el TextField
    private void ProcessAudio()
    {
        UnityEngine.Debug.Log("Process");
        if (audioToTranscript != null) {
            UnityEngine.Debug.Log("Processing...");

            // Llamar al metodo de Pyhton
            //var pythonSRT = 3;
            string venvPath = Path.GetFullPath("myENV"); // Carpeta del entorno virtual
            string pythonExe = Path.Combine(venvPath, "Scripts", "python.exe"); // Python del entorno virtual
            string scriptDir = Path.GetFullPath("./Assets/SubtitleGenerator");   // Carpeta donde está el script de Python
            string scriptName = "PyannoteWhisper.py";
            string audioPath = Path.GetFullPath(AssetDatabase.GetAssetPath(audioToTranscript));

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
            RunCommand(pythonExe, $"\"{scriptName}\" \"{audioPath}\"", scriptDir);

            // Podemos dejar definida la carpeta donde se va a encontrar el SRT hardcodeado
            // O podemos intentar sacar el output de python pero creo que eso te saca toda la consola y son muchas cosas

            // Método que expondrá en la ventana los dialogos
            ExposeTranscriptElements();

            // Escribir en el apartado del texto
            //transcriptText.value = "escribir lo de Python";

            // Rellenar el dropdown de actores con la informacion pertinente
            //List<string> testList = new List<string> { "Opción A", "Opción B", "Opción C" };
            //FillActors(testList);

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
        UnityEngine.Debug.Log("Hola");
        VisualTreeAsset dialogLineAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/SubtitleGenerator/Editor/Window/TranscriptDialogLine.uxml");

        // TO-DO -> que se añadan los verdaderos rellenando la info real
        for (int i = 0; i < 3; i++)
        {
            UnityEngine.Debug.Log(scrollView);
            VisualElement newDialog = dialogLineAsset.CloneTree();
            scrollView.Add(newDialog);
        }

    }

    // Rellenara los actores segun la lista recibida
    private void FillActors(List<string> actors)
    {
        actorsFoldout.Clear();

        foreach (string actor in actors) {
            var actorTextField = new TextField(actor);
            actorsFoldout.Add(actorTextField);
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
}
