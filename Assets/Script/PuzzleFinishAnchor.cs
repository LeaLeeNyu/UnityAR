using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class PuzzleFinishAnchor : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private AnchorDataSO anchorData;
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private Text debugText;

    private void OnEnable()
    {
        CubeCollision.enterRange.AddListener(InstantiateFinishAnchor);
    }

    private void OnDisable()
    {
        CubeCollision.enterRange.RemoveListener(InstantiateFinishAnchor);
    }

    private void InstantiateFinishAnchor()
    {
        // Instantiate the model 
        UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion(anchorData.quaternion.x, anchorData.quaternion.y, anchorData.quaternion.z, anchorData.quaternion.w);
        ARGeospatialAnchor anchor = ARAnchorManagerExtensions.AddAnchor(anchorManager, anchorData.latitude, anchorData.longitude, anchorData.altitude, quaternion);
        Instantiate(anchorPrefab, anchor.transform);
        debugText.text = "Anchor: Finish Anchor instantiated!";
    }
}
