using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnchorCollision: MonoBehaviour
{
    Renderer CubeRenderer;
    Vector3 PlayerForwardVector;
    Vector3 CubeForwardVector;
    bool _inRange = false;
    bool _run = false;
    public bool IsOverlapping { get { return isOverlapping(); } }

    // Use dot product to decide whether the player enter the range
    private float dotProduct;
    private GameObject player;

    //Debug & UI
    private Text anchorDebug;

    private void OnEnable()
    {
        CubeRenderer = GetComponent<Renderer>();
        player = GameObject.Find("AR Camera");
        anchorDebug = GameObject.Find("Anchor").GetComponent<Text>();       
    }

    void Update()
    {
        PlayerForwardVector = player.transform.forward;
        CubeForwardVector = this.transform.forward;
        dotProduct = Vector3.Dot(PlayerForwardVector, CubeForwardVector);
        GeospatialManager.Instance.DebugDot(dotProduct);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && gameObject.transform.position != Vector3.zero)
        {
            //player gets into collider, cube turns green
            CubeRenderer.material.color = new Color(0, 255, 0);
            //PlayerForwardVector = other.transform.forward;
            CubeForwardVector = this.transform.forward;
            _inRange = true;

            anchorDebug.text = gameObject.transform.position.ToString();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {    //player gets out of collider, cube turns red
            CubeRenderer.material.color = new Color(255, 0, 0);
            _inRange = false;
            anchorDebug.text = "Player Exit";
        }
    }

    private bool isOverlapping()
    {
        if(player != null)
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
        else
        {
            return false;
        }    
    }

    
}
