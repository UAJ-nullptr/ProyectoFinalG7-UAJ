using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System;
using static SubtitleManager;
using System.Globalization;

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

    List<SubtitleInfo> subtitles = new List<SubtitleInfo>();
    #endregion

    #region methods
    public void resetTime() { time = 0; }
    public void setTime(float nTime) { time = nTime; }
    public float getTime() { return time; }

    public void resetCont() { cont = 0; }
    public void setCont(int nCont) { cont = nCont; }
    public float getCont() { return cont; }

    public void readTextSRT()
    {
        StreamReader reader = new StreamReader(instance.path);

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
                    TimeSpan time = TimeSpan.ParseExact(parts[0].Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    int totalMilliseconds = (int)time.TotalMilliseconds;
                    subtitleInfo.startTime = totalMilliseconds;
                    time = TimeSpan.ParseExact(parts[1].Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
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
        StreamReader reader = new StreamReader(instance.path);

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
        //readTextJSON();
        //readTextWebVTT();

        //SubtitleInfo subtitleInfo = new SubtitleInfo();

        //subtitleInfo.talker = "";
        //subtitleInfo.content = "Hola esto es una prueba";
        //subtitleInfo.startTime = "0";
        //subtitleInfo.endTime = "5";

        //subtitles.Add(subtitleInfo);

        //subtitleInfo.talker = "";
        //subtitleInfo.content = "y continua";
        //subtitleInfo.startTime = "5";
        //subtitleInfo.endTime = "10";

        //subtitles.Add(subtitleInfo);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        // Comprobamos que inicia el hablante
        if (cont < subtitles.Count && subtitles[cont].startTime < time * 1000)
        {
            if(cont+1 < subtitles.Count && subtitles[cont+1].startTime < time * 1000) // Dos hablantes
            {
                // Descativamos el texto individual
                subtitleComponent.gameObject.SetActive(false);
                // Colocamos los nuevos textos para los dos hablantes
                subtitleComponentSpeaker1.setText(subtitles[cont]);
                subtitleComponentSpeaker2.setText(subtitles[cont+1]);
                // Activamos los textos de los dos hablantes
                subtitleComponentSpeaker1.gameObject.SetActive(true);
                subtitleComponentSpeaker2.gameObject.SetActive(true);
                cont++; 
            }
            else // Un solo hablante
            {
                // Desactivamos los textos de los dos hablantes
                subtitleComponentSpeaker1.gameObject.SetActive(false);
                subtitleComponentSpeaker2.gameObject.SetActive(false);
                // Colocamos el nuevo texto
                subtitleComponent.setText(subtitles[cont]);
                // Activamos el texto individual
                subtitleComponent.gameObject.SetActive(true);
            }
            cont++;
        }

        // Si se ha terminado el texto y se ha pasado el tiempo se desactivan los subtítulos
        if(cont > 0 && cont >= subtitles.Count && subtitles[cont-1].endTime < time * 1000)
        {
            subtitleComponent.gameObject.SetActive(false);
            subtitleComponentSpeaker1.gameObject.SetActive(false);
            subtitleComponentSpeaker2.gameObject.SetActive(false);
        }
    }
}
