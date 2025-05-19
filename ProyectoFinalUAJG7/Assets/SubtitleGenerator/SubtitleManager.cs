using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System;
using static SubtitleManager;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;


public class SubtitleManager : MonoBehaviour
{
    public static SubtitleManager instance { get; private set; }

    public struct SubtitleInfo
    {
        public string talker;
        public string content;
        //public string startTime;
        public int startTime;
        //public string endTime;
        public int endTime;
    }

    [System.Serializable]
    public class JSONformat
    {
        public string id;
        public string start;
        public string end;
        public string text;
    }

    [System.Serializable]
    public class Root
    {
        public string text;
        public List<JSONformat> segments;
    }

    #region atributes
    [SerializeField]
    string path = "prueba.txt";
    StreamReader reader;

    [SerializeField]
    private SubtitleComponent subtitleComponent;
    [SerializeField]
    private SubtitleComponent subtitleComponentSpeaker1;
    [SerializeField]
    private SubtitleComponent subtitleComponentSpeaker2;

    float time = 0;
    int cont = 0;
    float maxEndTime = 0;

    List<SubtitleInfo> subtitles = new List<SubtitleInfo>();

    private bool subtitlesActivated;
    #endregion

    #region methods
    public void ChangeState() { subtitlesActivated = !subtitlesActivated; }
    public void resetTime() { time = 0; }
    public void setTime(float nTime) { time = nTime; }
    public float getTime() { return time; }

    public void resetCont() { cont = 0; }
    public void setCont(int nCont) { cont = nCont; }
    public float getCont() { return cont; }

    List<SubtitleInfo> SplitPhrases(string fullText, int startTime, int endTime)
    {
        // Separar los fragmentos utilizando signos de puntuación
        List<string> sentences = Regex.Split(fullText, @"(?<=[.!?])\s+").ToList();

        // Calculamos las palabras de cada frase para tratar de ajustar el texto al sonido
        List<int> wordCounts = sentences.Select(s => s.Split(' ').Length).ToList();
        float totalWords = wordCounts.Sum();

        // En función de la longitud de la frase antes calculada asignamos la duración
        int currentStart = startTime;
        List<SubtitleInfo> entries = new List<SubtitleInfo>();

        for (int i = 0; i < sentences.Count; i++)
        {
            float fraction = wordCounts[i] / totalWords;
            int duration = (int)((endTime - startTime) * fraction);
            int currentEnd = currentStart + duration;

            entries.Add(new SubtitleInfo {
                content = sentences[i].Trim(),
                startTime = currentStart,
                endTime = currentEnd
            });

            currentStart = currentEnd;
        }

        return entries;
    }

    public void readTextSRT()
    {
        StreamReader reader;

        try
        {
            reader = new StreamReader(instance.path);
        }
        catch (Exception) {
            Debug.LogError("Archivo no encontrado: " + instance.path);
            return;
        }

        SubtitleInfo subtitleInfo = new SubtitleInfo();
        subtitleInfo.startTime = 0;
        subtitleInfo.endTime = 0;
        subtitleInfo.content = "";
        subtitleInfo.talker = "";

        String line;
        // Procesamos línea por línea
        while ((line = reader.ReadLine()) != null)
        {
            // Si la línea contiene --> tiene startTime y endTime
            if (line.Contains("-->"))
            {
                string[] parts = line.Split(new string[] { " --> " }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    // Cambio de formato para el tiempo
                    TimeSpan time = TimeSpan.ParseExact(parts[0].Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    int totalMilliseconds = (int)time.TotalMilliseconds;
                    subtitleInfo.startTime = totalMilliseconds;
                    time = TimeSpan.ParseExact(parts[1].Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    totalMilliseconds = (int)time.TotalMilliseconds;
                    subtitleInfo.endTime = totalMilliseconds;
                }
            }
            else if (line.Contains("Speaker")) // Si la línea contiene Speaker se asigna speaker
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {
                    if (subtitleInfo.endTime - subtitleInfo.startTime > 10000)
                    {
                        string talker = line.Substring(0, colonIndex).Replace("Speaker", "").Trim();
                        List<SubtitleInfo> listAux = SplitPhrases(line.Substring(colonIndex + 1).Trim(), subtitleInfo.startTime, subtitleInfo.endTime);

                        for (int i = 0; i < listAux.Count - 1; i++)
                        {
                            SubtitleInfo info = listAux[i];
                            info.talker = talker;
                            subtitles.Add(info);
                        }
                        // Se hace está última asignación porque el último fragmento se inserta con normalidad al final
                        subtitleInfo.talker = talker;
                        subtitleInfo = listAux[listAux.Count - 1];
                    }
                    else
                    {
                        subtitleInfo.talker = line.Substring(0, colonIndex).Replace("Speaker", "").Trim();
                        subtitleInfo.content = line.Substring(colonIndex + 1).Trim();
                    }               
                }
            }
            else if (line != "\n" && line.Length > 1) // Si no contiene el tiempo ni es un salto de línea es el contenido
            {
                subtitleInfo.content = line;
            }
            else if (line == "")
            {
                // Se añade el segmento a la lista de súbtitulos
                subtitles.Add(subtitleInfo);
            }           
        }
        subtitles.Add(subtitleInfo);
        reader.Close();
    }

    public void readTextJSON()
    {
        // Se lee el JSON si existe
        if (File.Exists(instance.path))
        {
            String json = File.ReadAllText(instance.path);
            Root root = JsonUtility.FromJson<Root>(json);

            SubtitleInfo subtitleInfo = new SubtitleInfo();
            // Se recorre por los segmentos procesados rellenando los campos y añadiendo los subtítulos
            foreach (var segment in root.segments)
            {
                subtitleInfo.talker = "";
                subtitleInfo.content = segment.text;
                float startAux = float.Parse(segment.start, CultureInfo.InvariantCulture.NumberFormat);
                subtitleInfo.startTime = (int)(startAux * 1000);
                float endAux = float.Parse(segment.end, CultureInfo.InvariantCulture.NumberFormat);
                subtitleInfo.endTime = (int)(endAux * 1000);

                subtitles.Add(subtitleInfo);
            }
        }
        else // No hay JSON
        {
            Debug.Log("JSON no encontrado: " + instance.path);
        }
    }

    public void readTextWebVTT()
    {
        try
        {
            StreamReader reader = new StreamReader(instance.path);
        }
        catch (Exception)
        {
            Debug.LogError("Archivo no encontrado: " + instance.path);
            return;
        }

        SubtitleInfo subtitleInfo = new SubtitleInfo();
        subtitleInfo.startTime = 0;
        subtitleInfo.endTime = 0;
        subtitleInfo.content = "";
        subtitleInfo.talker = "";

        String line;
        // Procesamos línea por línea
        while ((line = reader.ReadLine()) != null)
        {
            // Si la línea contiene --> tiene startTime y endTime
            if (line.Contains("-->"))
            {
                string[] parts = line.Split(new string[] { " --> " }, StringSplitOptions.None);
                if (parts.Length == 2)
                {
                    //subtitleInfo.startTime = parts[0].Trim();
                    //subtitleInfo.endTime = parts[1].Trim();

                    // Cambio de formato
                    TimeSpan time = TimeSpan.ParseExact(parts[0].Trim(), @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
                    int totalMilliseconds = (int)time.TotalMilliseconds;
                    subtitleInfo.startTime = totalMilliseconds;
                    time = TimeSpan.ParseExact(parts[1].Trim(), @"hh\:mm\:ss\.fff", CultureInfo.InvariantCulture);
                    totalMilliseconds = (int)time.TotalMilliseconds;
                    subtitleInfo.endTime = totalMilliseconds;
                }
            }
            else if (line != "\n" && line.Length > 1) // Si no contiene el tiempo ni es un salto de línea es el contenido
            {
                subtitleInfo.content = line;
            }
            else if (line == "")
            {
                // Se añade el segmento a la lista de súbtitulos
                subtitles.Add(subtitleInfo);
            }
        }
        subtitles.Add(subtitleInfo);
        reader.Close();
    }
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        readTextSRT();
        subtitlesActivated = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        time += Time.deltaTime;
        // Comprobamos que inicia el hablante
        if (cont < subtitles.Count && subtitles[cont].startTime < time * 1000)
        {
            if(cont+1 < subtitles.Count && subtitles[cont+1].startTime < subtitles[cont].endTime) // Dos hablantes
            {
                if (subtitlesActivated)
                {
                    // Descativamos el texto individual
                    subtitleComponent.gameObject.SetActive(false);

                    // Activamos los textos de los dos hablantes
                    subtitleComponentSpeaker1.gameObject.SetActive(true);
                    subtitleComponentSpeaker2.gameObject.SetActive(true);

                    // Colocamos los nuevos textos para los dos hablantes
                    subtitleComponentSpeaker1.setText(subtitles[cont]);
                    subtitleComponentSpeaker2.setText(subtitles[cont + 1]);
                }
                maxEndTime = Math.Max(subtitles[cont].endTime, subtitles[cont+1].endTime);
                cont++; 
            }
            else // Un solo hablante
            {
                if (subtitlesActivated)
                {
                    // Desactivamos los textos de los dos hablantes
                    subtitleComponentSpeaker1.gameObject.SetActive(false);
                    subtitleComponentSpeaker2.gameObject.SetActive(false);

                    // Activamos el texto individual
                    subtitleComponent.gameObject.SetActive(true);

                    // Colocamos el nuevo texto
                    subtitleComponent.setText(subtitles[cont]);
                }
                maxEndTime = subtitles[cont].endTime;
            }
            cont++;
        }

        // Si se ha terminado el texto y se ha pasado el tiempo se desactivan los subtítulos
        if(cont > 0 && cont >= subtitles.Count && maxEndTime < time * 1000)
        {
            subtitleComponent.gameObject.SetActive(false);
            subtitleComponentSpeaker1.gameObject.SetActive(false);
            subtitleComponentSpeaker2.gameObject.SetActive(false);
        }
    }
}
