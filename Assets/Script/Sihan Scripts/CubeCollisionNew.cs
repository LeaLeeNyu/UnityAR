using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CubeCollisionNew : MonoBehaviour
{
    Renderer CubeRenderer;
    Vector3 PlayerForwardVector;
    Vector3 CubeForwardVector;
    private bool inRange = false;
    private float dotProduct;
    private GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //player gets into collider, cube turns green
            CubeRenderer.material.color = new Color(0, 255, 0);
            player = other.gameObject;
            inRange = true; 
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {    //player gets out of collider, cube turns red
            CubeRenderer.material.color = new Color(255, 0, 0);
        }
    }

    private void Update()
    {
        if (inRange)
        {
            PlayerForwardVector = player.transform.forward;
            CubeForwardVector = this.transform.forward;
            dotProduct =Vector3.Dot(PlayerForwardVector, CubeForwardVector);
            if (dotProduct > 0.9)
            {
                CubeRenderer.material.color = new Color(255, 255, 255);
            }
        }

    }
}
