using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CalculateDistance : MonoBehaviour
{

   [SerializeField] private AnchorDataSO[] anchorData;
    [SerializeField] private GameObject notification;
    [SerializeField] TMP_Text debugText;

    Vector2 userLocation;
    Vector2[] anchorCoordinates;
    private bool hasMoved=false;
    void Start()
    {
        anchorCoordinates=new Vector2[anchorData.Length];
        for (int i = 0; i < anchorData.Length; i++)
        {
            anchorCoordinates[i]= new Vector2((float)anchorData[i].latitude, (float)anchorData[i].longitude);
        }

       //anchorCoordinates = new Vector2((float)anchorData.latitude, (float)anchorData.longitude);
    }

    void Update()
    {
        //if(Input.GetKeyDown("space") && hasMoved == false) {
            //notification.GetComponent<UIPanalMovement>().MoveDown();
           // hasMoved = true;
       // }

        if(Input.location.status == LocationServiceStatus.Running&&hasMoved==false)
        {
            userLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);

            for (int i = 0; i < anchorCoordinates.Length; i++)
            {
                float distance = OnlineMapsUtils.DistanceBetweenPoints(userLocation, anchorCoordinates[i]).magnitude;
                //debugText.text = distance.ToString();
                if (distance < 0.005f)
                {
                    notification.GetComponent<UIPanalMovement>().MoveDown();
                    hasMoved = true;
                }
            }
        }
    }


}
