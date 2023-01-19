using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanalMovement : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRectTransform;
    RectTransform rectTransform;

    private void OnEnable()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform=canvas.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
       
    }

    public void MoveUp()
    {
        transform.LeanMoveY(transform.position.y + rectTransform.rect.height * canvasRectTransform.localScale.y, 1).setEaseInOutQuad();
    }

    public void MoveDown()
    {
        transform.LeanMoveY(transform.position.y - rectTransform.rect.height * canvasRectTransform.localScale.y, 1).setEaseInOutQuad();
    }
}
