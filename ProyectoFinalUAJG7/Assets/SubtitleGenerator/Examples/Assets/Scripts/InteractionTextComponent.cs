using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Properties;
using UnityEngine;

public class InteractionTextComponent : MonoBehaviour
{
    [SerializeField] InteractionSystem interactionSystem;
    TMP_Text text;

    private void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactionSystem != null)
        {
            text.alpha = interactionSystem.interactionAvailable? 1 : 0;
        }
    }
}
