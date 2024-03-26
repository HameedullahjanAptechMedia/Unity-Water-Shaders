using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDragging : MonoBehaviour
{
 
    private Vector3 offset;
    private Vector3 originalPosition;
    private bool isDragging;
    public float dragThreshold = 0.1f; // Adjust as needed

    private void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            OnMouseDrag();
         } 
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            OnMouseDown();
         }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            OnMouseUp();
         }
    }
    void OnMouseDown()
    {
        originalPosition = transform.position;
        offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
        isDragging = false;
    }

    void OnMouseDrag()
    {
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f)) + offset;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        if (!isDragging && Vector3.Distance(originalPosition, transform.position) > dragThreshold)
        {
            isDragging = true;
            Debug.Log("Started Dragging");
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            Debug.Log("Stopped Dragging");
        }
    }
 
}
