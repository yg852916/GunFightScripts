using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
public class ButtonEvent : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent useEvent;
    public UnityEvent dropEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            useEvent.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            dropEvent.Invoke();
        }


    }
}
