using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CubeCollision : MonoBehaviour
{
    Renderer CubeRenderer;
    Vector3 PlayerForwardVector;
    Vector3 CubeForwardVector;
    bool _inRange = false;
    bool _run = false;

    // Use dot product to decide whether the player enter the range
    private float dotProduct;
    private GameObject player;

    //When players enter the range, invoke the enter event
    [HideInInspector] public static UnityEvent enterRange;

    void Start()
    {
        CubeRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        isOverlapping();
        GeospatialManager.Instance.DebugDot(dotProduct);

    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //player gets into collider, cube turns green
            CubeRenderer.material.color = new Color(0, 255, 0);
            //PlayerForwardVector = other.transform.forward;
            player = other.gameObject;
            CubeForwardVector = this.transform.forward;
            _inRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {    //player gets out of collider, cube turns red
            CubeRenderer.material.color = new Color(255, 0, 0);
            _inRange = false;
        }
    }

    private bool isOverlapping()
    {
        if(player != null)
        {
            PlayerForwardVector = player.transform.forward;
            dotProduct = Vector3.Dot(PlayerForwardVector, CubeForwardVector);

            //if the angle between two vectors is less than 30 degrees, cube turns white
            if (_inRange && dotProduct > 0.8)
            {
                CubeRenderer.material.color = new Color(255, 255, 255);
                return true;
            }
            else if (_inRange)
            {
                // if player in the range but not makes picture overlap, turn  blue
                CubeRenderer.material.color = new Color(0, 255, 0);
                return false;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }    
    }
}
