using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Draggable : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void EmptyDelegate();

    public event EmptyDelegate DragStarted;
    public event EmptyDelegate DragEnded;

    private Vector3 _mouseDragStartPosition;
    private Vector3 _draggableStartPosition;

    private bool _isDragging = false;


    private void Update()
    {
        if (_isDragging)
        {
            transform.localPosition = _draggableStartPosition - _mouseDragStartPosition + Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        print(eventData);
        _isDragging = true;
        DragStarted();
        _mouseDragStartPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _draggableStartPosition = transform.localPosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isDragging = false;
        DragEnded();
    }
}
