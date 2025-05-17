using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;



public struct Line
{
    public String actorKey;
    public float startTime;
    public float endTime;

    public String line;

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
        actors = new();
        lines = new();
    }


    public Dictionary<String, Actor> actors;

    public List<Line> lines;
}



public class DialogueManager
{
    //generar objetos 
    
    public Dialogue? ReadTextSRT(string path)
    {
        Dialogue dialogue = new Dialogue();

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

        

        String line;
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
                                      
                    String actorKey = line.Substring(0, colonIndex).Replace("Speaker", "").Trim();

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
}
