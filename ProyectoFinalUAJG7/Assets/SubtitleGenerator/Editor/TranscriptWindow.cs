using Codice.CM.Common.Tree;
using System;
using System.Collections;
using System.Collections.Generic;
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

    // Audio a procesar
    private AudioClip audioToTranscript;

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

        // Asignar callbacks
        audioFileInput.RegisterValueChangedCallback(AudioSelected);
        processButton.clicked += ProcessAudio;
        saveButton.clicked += SaveTranscript;
        exportButton.clicked += ExportTranscript;
        deleteButton.clicked += DeleteOptions;

        Debug.Log("GUI created.");
    }

    // Metodo para cuando se añade el audio
    private void AudioSelected(ChangeEvent<UnityEngine.Object> evt)
    {
        Debug.Log("Audio");
        string path = AssetDatabase.GetAssetPath((AudioClip)evt.newValue);
        if (!path.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
        {
            Debug.LogWarning("Only .wav files are allowed to be transcribed.");
            audioToTranscript = null;
        }
        else
        {
            audioToTranscript = (AudioClip)evt.newValue;
            Debug.Log("\"" + audioToTranscript.name + "\" setted correctly.");
        }
    }

    // Metodo que llama a Wisper y compañia para entonces mostrarlo en el TextField
    private void ProcessAudio()
    {
        Debug.Log("Process");
        if (audioToTranscript != null) {
            Debug.Log("Processing...");
            // Llamar al metodo de Pyhton

            // Escribir en el apartado del texto
            transcriptText.value = "escribir lo de Python";

            // Rellenar el dropdown de actores con la informacion pertinente
            List<string> testList = new List<string> { "Opción A", "Opción B", "Opción C" };
            FillActors(testList);

            Debug.Log("Processed");
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
        Debug.Log("Save");
        if (transcriptText.value != null)
        {
            // Guardar en un archivo?
        }
    }

    // Exportar la transcripcion
    private void ExportTranscript()
    {
        Debug.Log("Export");
        if (transcriptText.value != null)
        {
            // Guardar en un archivo... otra vez?
        }
    }

    // Setea la interfaz y su info a los valores iniciales
    private void DeleteOptions()
    {
        Debug.Log("Delete");
        transcriptText.value = null;
        audioToTranscript = null;
    }
}
