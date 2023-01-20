using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PuzzleShowModel : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;

    [SerializeField] private AnchorDataSO anchorData;
    [SerializeField] private GameObject anchorPrefab;

    //Debug
    [SerializeField] private Text anchorDebug;

    private bool _run = false;

    private void Start()
    {
        // if Geospatial finish initialize
        GeospatialManager.Instance.InitCompleted.AddListener(OnGeoInitCompleted);

        anchorDebug.text = "Anchor: No Anchor!";
    }

    //Instantiate the start anchor
    private void OnGeoInitCompleted()
    {
        GeospatialManager.Instance.InitCompleted.RemoveListener(OnGeoInitCompleted);

        UnityEngine.Quaternion quaternion = new UnityEngine.Quaternion(anchorData.quaternion.x, anchorData.quaternion.y, anchorData.quaternion.z, anchorData.quaternion.w);
        ARGeospatialAnchor anchor = ARAnchorManagerExtensions.AddAnchor(anchorManager, anchorData.latitude, anchorData.longitude, anchorData.altitude, quaternion);
        Instantiate(anchorPrefab, anchor.transform);
        anchorDebug.text = "Anchor: Instantiate Finished!";
    }
}
