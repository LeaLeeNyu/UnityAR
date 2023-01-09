using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    Renderer CubeRenderer;
    Vector3 PlayerForwardVector;
    Vector3 CubeForwardVector;
    bool isComparingVector=false;
    
    void Start()
    {
        CubeRenderer=GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isComparingVector)
        {
            //if the angle between two vectors is less than 30 degrees, cube turns white
            if(Vector3.Dot(PlayerForwardVector, CubeForwardVector) > 0.85)
            {
                CubeRenderer.material.color = new Color(255, 255, 255);
            }
            else
            {
                CubeRenderer.material.color = new Color(0, 255, 0);
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //player gets into collider, cube turns green
            CubeRenderer.material.color = new Color(0, 255, 0);
            PlayerForwardVector = other.transform.forward;
            CubeForwardVector = this.transform.forward;
            isComparingVector = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {    //player gets out of collider, cube turns red
            CubeRenderer.material.color = new Color(255, 0, 0);
            isComparingVector = false;
        }
    }
}
