using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class CheckPosManager : MonoBehaviour
{
    [SerializeField] private string photoAnchorName = "PhotoAnchor";

    //UI
    [SerializeField] private GameObject nullAnchorUI;
    [SerializeField] private GameObject completeUI;
    [SerializeField] private GameObject failUI;
    private bool UIOpen = false;

    //Debug
    [SerializeField] private Text anchorDebug;

    //When players enter the range, invoke the enter event
    [HideInInspector] public static UnityEvent enterRange;

    //Model 
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private AnchorDataSO anchorData;
    [SerializeField] private GameObject anchorPrefab;

    public void PlayerTakePhoto()
    {
        anchorDebug.text = "Player take photo";

        if (UIOpen)
        {
            anchorDebug.text = "Please close the UI";
        }
        else
        {
            anchorDebug.text = "UI is not open";
        }

        

        // if player take a photo
        // check if the cube finished instantiated
        GameObject[] anchor = GameObject.FindGameObjectsWithTag("FirstAnchor");
        AnchorCollision anchorScript = anchor[0].GetComponent<AnchorCollision>();

        anchorDebug.text = anchorScript.IsOverlapping.ToString();

        if (anchor == null)
        {
            nullAnchorUI.SetActive(true);
            UIOpen = true;
        }

        // if player overlap the photo with the real sight
        if (anchorScript.IsOverlapping)
        {
            completeUI.SetActive(true);
            UIOpen = true;

            GameObject.Destroy(anchorScript.gameObject);
            //ModelInstantiate();
            anchorDebug.text = "compelet photo & Instantiate model";           
        }
        else
        {
            failUI.SetActive(true);
            UIOpen = true;
        }
    }

    public void UIToggle()
    {
        UIOpen = false;
    }


    //Instantiate model
    private void ModelInstantiate()
    {
        UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion(anchorData.quaternion.x, anchorData.quaternion.y, anchorData.quaternion.z, anchorData.quaternion.w);
        ARGeospatialAnchor anchor = ARAnchorManagerExtensions.AddAnchor(anchorManager, anchorData.latitude, anchorData.longitude, anchorData.altitude, quaternion);
        Instantiate(anchorPrefab, anchor.transform);
        anchorDebug.text = "Anchor: Finish Anchor instantiated!";
    }

}
