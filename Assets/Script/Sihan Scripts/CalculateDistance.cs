using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateDistance : MonoBehaviour
{
    Vector2 userCoordinates;
    Vector2 markerCoordinates;

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 userCoordinates= new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            float distance = OnlineMapsUtils.DistanceBetweenPoints(userCoordinates, markerCoordinates).magnitude;
        }
    }
}
