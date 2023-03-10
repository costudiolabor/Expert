using UnityEngine;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    [SerializeField] private RectTransform rectTransform;
    private Vector2 _offSet;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var position = rectTransform.position;
        _offSet = eventData.pointerCurrentRaycast.screenPosition - new Vector2(position.x, position.y);
    }

    public void OnDrag(PointerEventData eventData) {
        
        rectTransform.position = eventData.pointerCurrentRaycast.screenPosition - _offSet;
    }

   
}