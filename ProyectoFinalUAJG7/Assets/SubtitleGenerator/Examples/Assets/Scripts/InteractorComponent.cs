using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractorComponent : MonoBehaviour
{
    public UnityEvent interactionEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        var iS = other.gameObject.GetComponent<InteractionSystem>();
        if (iS != null)
        {
            iS.register(this);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        var iS = other.gameObject.GetComponent<InteractionSystem>();
        if (iS != null)
        {
            iS.unregister(this);
        }

    }

    public void fireInteraction()
    {
        interactionEvent?.Invoke();
    }


}
