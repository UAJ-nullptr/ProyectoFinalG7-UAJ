using Codice.CM.Client.Differences.Graphic;
using Codice.CM.Common.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.Video;
using static SubtitleManager;
using Debug = UnityEngine.Debug;

public class TranscriptWindow : EditorWindow
{
    // Elementos de la UI de la ventana en orden de aparicion
    private UnityEditor.UIElements.ObjectField audioFileInput;
    private UnityEngine.UIElements.Button processButton;
    private UnityEditor.UIElements.ObjectField fileInfoInput;
    private Foldout actorsFoldout;
    private UnityEngine.UIElements.Button saveButton;
    private UnityEngine.UIElements.Button exportButton;
    private ScrollView scrollView;

    // Audio a procesar
    private AudioClip audioToTranscript;
    private VideoClip videoToTranscript;

    // Proceso de informacion
    private DialogueManager dialogueManager;
    private Dialogue currentDiag;
    private List<TranscriptDialogueLine> transcriptDialogueList;
    private SubtitleData subtitleData;


    // Añadir al menú contextual y abrir ventana
    // Se hace en "Tools" porque Unity obliga a que sea en esa pestaña por consistencia
    // en la AssetStore
    [MenuItem("Tools/Transcript Audio Tool")]
    public static void OpenEditorWindow()
    {
        TranscriptWindow window = GetWindow<TranscriptWindow>();
        window.titleContent = new GUIContent("Transcript Audio Tool");
        window.maxSize = new Vector2(900, 600);
        window.minSize = window.maxSize;
    }

    // Crear la ventana a partir del UXMl y obtener los elementos
    public void CreateGUI()
    {
        // Crear la GUI
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
            "Assets/SubtitleGenerator/Scripts/Editor/Window/SubEditorWindow.uxml");
        VisualElement tree = visualTree.Instantiate();
        root.Add(tree);

        // Guardar los elementos
        audioFileInput = root.Q<UnityEditor.UIElements.ObjectField>("audioField");
        processButton = root.Q<UnityEngine.UIElements.Button>("process");
        fileInfoInput = root.Q<UnityEditor.UIElements.ObjectField>("fileInfo");
        fileInfoInput.objectType = typeof(SubtitleData);
        actorsFoldout = root.Q<Foldout>("actors");
        saveButton = root.Q<UnityEngine.UIElements.Button>("saveButton");
        exportButton = root.Q<UnityEngine.UIElements.Button>("exportButton");
        scrollView = root.Q<ScrollView>("transcriptElements");

        // Asignar callbacks
        audioFileInput.RegisterValueChangedCallback(AudioSelected);
        fileInfoInput.RegisterValueChangedCallback(FileLoaded);
        processButton.clicked += ProcessAudio;

        saveButton.clicked += SaveTranscript;
        exportButton.clicked += ExportTranscript;

        dialogueManager = new DialogueManager();
        transcriptDialogueList = new List<TranscriptDialogueLine>();

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

    private void FileLoaded(ChangeEvent<UnityEngine.Object> evt)
    {
        var eventData = evt.newValue;
        if (eventData is SubtitleData)
        {
            subtitleData = (SubtitleData)eventData;
            currentDiag = subtitleData.dialogue;
            var actorsNameList = PopulateActorsNameList();
            PopulateTranscriptLineList(actorsNameList);
        }
        else
        {
            Debug.LogError("Only SubtitleData ScriptableObjects may be used here");
        }
    }

    // Metodo que llama a Whisper y compañia para entonces mostrarlo en el TextField
    private async void ProcessAudio()
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

            string pythonCmd = $"{scriptName}"; /*{arguments}*/

            // Correr el archivo de python
            string transPath = await RunCommandAsync(pythonExe, $"\"{scriptName}\" \"{inputPath}\"", scriptDir);

            // Método que expondrá en la ventana los dialogos
            ExposeTranscriptElements(transPath);

            UnityEngine.Debug.Log("Processed");
        }
        else
        {
            UnityEngine.Debug.LogWarning("There is no file to transcript: please select an audio/video in the 'Audio File' or 'Subtitle Object' field for it to be processed");
        }
    }

    public async Task<string> RunCommandAsync(string executer, string command, string workingDirectory, bool useShell = false)
    {
        using (Process process = new Process())
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = executer;
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // Lectura no bloqueante
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await Task.Run(() => process.WaitForExit());

            List<string> result = new List<string>(output.Split(new[] { '\n', '\r' }, StringSplitOptions.None));

            return result[result.Count - 3];
        }
    }

    // Llamar desde otro script, o usar un botón para probar
    public async void Start()
    {
        string result = await RunCommandAsync("tuEjecutable", "--argumentos", "ruta/al/directorio");
        Debug.Log("Resultado del proceso: " + result);
    }

    private void ExposeTranscriptElements(string srtPath)
    {
        UnityEngine.Debug.Log("Mostrando texto obtenido");
        currentDiag = (Dialogue) dialogueManager.ReadTextSRT(srtPath);

        // Foldout de actores
        VisualTreeAsset actorsAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/SubtitleGenerator/Scripts/Editor/Window/ActorsFoldout.uxml");

        List<string> actorsNamesList = PopulateActorsNameList();

        PopulateTranscriptLineList(actorsNamesList);
    }

    private List<string> PopulateActorsNameList()
    {
        actorsFoldout.Clear();
        List<string> actorsNamesList = new List<string>();
        foreach (var actor in currentDiag.actors)
        {
            Debug.Log("hola");
            ActorsFoldout newActor = CreateInstance<ActorsFoldout>();
            newActor.PopulateActorFoldout(currentDiag, actor.Key);
            newActor.CreateGUI();
            newActor.SetWindow(this);

            actorsFoldout.Add(newActor.rootVisualElement);
            actorsNamesList.Add(actor.Key);
        }

        return actorsNamesList;
    }

    private void PopulateTranscriptLineList(List<string> actorsNamesList)
    {
        // Lista de transcripción
        scrollView.Clear();
        transcriptDialogueList.Clear();
        foreach (var line in currentDiag.lines)
        {
            TranscriptDialogueLine newDiag = CreateInstance<TranscriptDialogueLine>();
            newDiag.PopulateDialogueLine(line, currentDiag, actorsNamesList);
            newDiag.CreateGUI();
            scrollView.contentContainer.Add(newDiag.rootVisualElement);
            transcriptDialogueList.Add(newDiag);
        }
    }

    // Guardar la transcripcion
    private void SaveTranscript()
    {
        UnityEngine.Debug.Log("Save");

        // Comprobar si se ha seleccionado algo a procesar
        if (!audioToTranscript && !videoToTranscript && !subtitleData)
        {
            UnityEngine.Debug.LogWarning("There is no file to transcript: please process audio/video before saving");
            return;
        }

        string folderPath = "Assets/SubtitleGenerator/Subtitles";
        // Comprueba si la carpeta existe o no y si no exite la crea
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            string aux = AssetDatabase.CreateFolder("Assets/SubtitleGenerator", "Subtitles");
            folderPath = AssetDatabase.GUIDToAssetPath(aux);
        }

        saveToFile(folderPath);
        createNewSubtitleData(folderPath);
        subtitleData.dialogue = currentDiag;
        subtitleData.dialogueAudio = null;
        fileInfoInput.value = subtitleData;
    }

    private void saveToFile(string folderPath)
    {
        string srtPath = folderPath + "\\" + (audioToTranscript ? audioToTranscript.name : videoToTranscript.name) + ".str";
        StreamWriter writer = new StreamWriter(srtPath);

        int index = 1;
        foreach (TranscriptDialogueLine tdl in transcriptDialogueList)
        {
            // Obtener la línea y escribir el index
            Line line = tdl.getLine();
            writer.WriteLine(index);

            // Escribir el tiempo formateado
            TimeSpan startTime = TimeSpan.FromMilliseconds(line.startTime);
            string startFormat = string.Format("{0:00}:{1:00}:{2:00},{3:000}",
                startTime.Hours,
                startTime.Minutes,
                startTime.Seconds,
                startTime.Milliseconds);
            TimeSpan endTime = TimeSpan.FromMilliseconds(line.endTime);
            string endFormat = string.Format("{0:00}:{1:00}:{2:00},{3:000}",
                startTime.Hours,
                startTime.Minutes,
                startTime.Seconds,
                startTime.Milliseconds);
            writer.WriteLine(startFormat + " --> " + endFormat);

            // Escribir el speaker y la línea
            writer.WriteLine("Speaker " + line.actorKey + ":  " + line.line);
            writer.WriteLine();

            index++;
        }
        writer.Close();
    }

    private void createNewSubtitleData(string folderPath)
    {
        
        SubtitleData newSD = CreateInstance<SubtitleData>();
        newSD.name = audioToTranscript ? audioToTranscript.name : videoToTranscript.name;

        // Create the asset
        string assetPath = Path.Combine(folderPath, newSD.name + ".asset");

        if (AssetDatabase.LoadAssetAtPath<SubtitleData>(assetPath) != null)
        {
            AssetDatabase.DeleteAsset(assetPath);
        }

        AssetDatabase.CreateAsset(newSD, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        SubtitleData loadedSD = AssetDatabase.LoadAssetAtPath<SubtitleData>(assetPath);
        subtitleData = loadedSD;
    }

    private void ExportTranscript()
    {
        UnityEngine.Debug.Log("Export");

        if (!audioToTranscript && !videoToTranscript && !subtitleData)
        {
            UnityEngine.Debug.LogWarning("There is no file to transcript: please process audio/video before saving");
            return;
        }

        string folderPath = EditorUtility.OpenFolderPanel("Choose Folder to Save New srt File and SubtitleData", "Assets", "");
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogWarning("Save canceled by user.");
            return;
        }

        // Convert absolute path to relative (Unity needs relative paths for AssetDatabase)
        if (folderPath.StartsWith(Application.dataPath))
        {
            folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);
        }
        else
        {
            Debug.LogError("Selected folder must be inside the Assets folder.");
            return;
        }

        saveToFile(folderPath);
        createNewSubtitleData(folderPath);
        subtitleData.dialogue = currentDiag;
        subtitleData.dialogueAudio = null;
        fileInfoInput.value = subtitleData;
    }

    public List<TranscriptDialogueLine> getTranscriptsList()
    {
        return transcriptDialogueList;
    }
}
