using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public struct Line
{
    public string actorKey;
    public float startTime;
    public float endTime;
    public string line;
}

public struct Actor
{
    public Actor(int unused = 0)
    {
        name = "";
        color = Color.white;
    }

    public string name;
    public Color color;
}

public struct Dialogue
{
    public Dialogue(int unused = 0)
    {
        actors = new Dictionary<string, Actor>();
        lines = new List<Line>();
    }

    public Dictionary<string, Actor> actors;

    public List<Line> lines;
}

public class DialogueManager
{
    //generar objetos 
    
    public Dialogue? ReadTextSRT(string path)
    {
        Dialogue dialogue = new Dialogue(0);

        StreamReader reader;

        try
        {
            reader = new StreamReader(path);
        }
        catch (Exception)
        {
            Debug.LogError("Archivo no encontrado: " + path);
            return null;
        }

        string line;
        // Procesamos línea por línea
        Line newLine = new();
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
                    newLine.startTime = totalMilliseconds;
                    time = TimeSpan.ParseExact(parts[1].Trim(), @"hh\:mm\:ss\,fff", CultureInfo.InvariantCulture);
                    totalMilliseconds = (int)time.TotalMilliseconds;
                    newLine.endTime = totalMilliseconds;
                }
            }
            else if (line.Contains("Speaker")) // Si la línea contiene Speaker se asigna speaker
            {
                int colonIndex = line.IndexOf(':');
                if (colonIndex > 0)
                {                 
                    if(newLine.endTime - newLine.startTime > 10000)
                    {
                        string talker = line.Substring(0, colonIndex).Replace("Speaker", "").Trim();
                        List<Line> listAux = SplitPhrases(line.Substring(colonIndex + 1).Trim(), (int)newLine.startTime, (int)newLine.endTime);

                        for (int i = 0; i < listAux.Count - 1; i++)
                        {
                            // Se añade el segmento a la lista de súbtitulos
                            newLine = listAux[i];
                            newLine.actorKey = talker;
                            dialogue.lines.Add(newLine);
                            newLine = new();
                        }
                        newLine = listAux[listAux.Count - 1];
                        newLine.actorKey = talker;
                    }
                    else
                    {
                        string actorKey = line.Substring(0, colonIndex).Replace("Speaker", "").Trim();

                        Actor assignedActor;

                        if (dialogue.actors.ContainsKey(actorKey))
                        {
                            assignedActor = dialogue.actors[actorKey];
                        }
                        else
                        {
                            assignedActor = new Actor();
                            dialogue.actors.Add(actorKey, assignedActor);
                        }

                        newLine.actorKey = actorKey;
                        newLine.line = line.Substring(colonIndex + 1).Trim();
                    }
                }
            }
            else if (line != "\n" && line.Length > 1) // Si no contiene el tiempo ni es un salto de línea es el contenido
            {
                newLine.line += " " + line;
            }
            else if (line == "")
            {
                // Se añade el segmento a la lista de súbtitulos
                dialogue.lines.Add(newLine);
                newLine = new();
            }
        }
        reader.Close();

        return dialogue;
    }

    List<Line> SplitPhrases(string fullText, int startTime, int endTime)
    {
        // Separar los fragmentos utilizando signos de puntuación
        List<string> sentences = Regex.Split(fullText, @"(?<=[.!?])\s+").ToList();

        // Calculamos las palabras de cada frase para tratar de ajustar el texto al sonido
        List<int> wordCounts = sentences.Select(s => s.Split(' ').Length).ToList();
        float totalWords = wordCounts.Sum();

        // En función de la longitud de la frase antes calculada asignamos la duración
        int currentStart = startTime;
        List<Line> entries = new List<Line>();

        for (int i = 0; i < sentences.Count; i++)
        {
            float fraction = wordCounts[i] / totalWords;
            int duration = (int)((endTime - startTime) * fraction);
            int currentEnd = currentStart + duration;

            entries.Add(new Line {
                line = sentences[i].Trim(),
                startTime = currentStart,
                endTime = currentEnd
            });

            currentStart = currentEnd;
        }

        return entries;
    }
}
