using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestBackEnd : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private RectTransform _image;
    
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("click");
        _image.position = eventData.position;
    }
}
