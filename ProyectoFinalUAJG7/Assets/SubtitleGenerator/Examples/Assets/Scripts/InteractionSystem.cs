using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractionSystem : MonoBehaviour
{
    List<InteractorComponent> interactions;

    public bool interactionAvailable = false;

    private void Start()
    {
        interactions = new List<InteractorComponent>();
    }

    private void Update()
    {
        if (interactions.Count != 0)
        {
            interactionAvailable = true;

            if (Input.GetKeyDown(KeyCode.E))
            {
                foreach (InteractorComponent component in interactions) { 
                    component.fireInteraction();
                }
            }
        }
        else {
            interactionAvailable = false;
        }
    }

    internal void register(InteractorComponent interactorComponent)
    {
        interactions.Add(interactorComponent);
    }

    internal void unregister(InteractorComponent interactorComponent)
    {
        interactions.Remove(interactorComponent);
    }


}
