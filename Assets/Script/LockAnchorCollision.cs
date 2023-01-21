using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockAnchorCollision : AnchorCollision
{
    protected override bool isOverlapping()
    {
        PlayerForwardVector = player.transform.forward;
        dotProduct = Vector3.Dot(PlayerForwardVector, CubeForwardVector);

        //if the angle between two vectors is less than 30 degrees, cube turns white
        if (_inRange && dotProduct > 0.99)
        {
            CubeRenderer.material.color = new Color(255, 255, 255);
            return true;
        }
        else
        {
            return false;
        }
    }
}
