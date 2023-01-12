using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanalMovement : MonoBehaviour
{
    private void OnEnable()
    {

        transform.LeanMoveY(transform.position.y + 1346, 1).setEaseInOutQuad();
    }
}
