using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class RestartBtnScript : MonoBehaviour, IPointerClickHandler
{
    public static event Action onRestart;
    public void OnPointerClick(PointerEventData eventData)
    {
        onRestart?.Invoke();
    }

    
}
