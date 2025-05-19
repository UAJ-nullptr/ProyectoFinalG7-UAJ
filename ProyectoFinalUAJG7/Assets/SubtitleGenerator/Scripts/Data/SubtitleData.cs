using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Dialogue;

public class SubtitleData : ScriptableObject
{

    public List<Line> lines;
    public List<string> actorKeys;
    public List<Actor> actorValues;

    public Dialogue getDialogue()
    {
        Dialogue dialogue = new Dialogue();
        dialogue.lines = lines;
        Dictionary<string,Actor> keyValuePairs = new Dictionary<string,Actor>();
        for (int i = 0; i < actorValues.Count; i++)
        {
            keyValuePairs.Add(actorKeys[i], actorValues[i]);
        }
        dialogue.actors = keyValuePairs;
        return dialogue;
    }

    public void saveDialogue(Dialogue dialogueToSave)
    {
        lines = dialogueToSave.lines;

        List<string> newActorKeys = new();
        List<Actor> newActorValues = new();

        foreach(var val in dialogueToSave.actors)
        {
            newActorKeys.Add(val.Key);
            newActorValues.Add(val.Value);
        }

        actorKeys = newActorKeys;
        actorValues = newActorValues;
    }

    public AudioClip dialogueAudio;
    
}
