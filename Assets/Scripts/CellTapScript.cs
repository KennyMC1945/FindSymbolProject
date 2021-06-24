using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellTapScript : MonoBehaviour, IPointerClickHandler
{
    public string symbol;
    public static event Action<string, GameObject> onCellTapped;

    public void OnPointerClick(PointerEventData eventData)
    {
        CellTapped();
    }

    public void CellTapped()
    {
        onCellTapped?.Invoke(symbol, gameObject);
    }


}
