using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class PuzzleStartAnchor : MonoBehaviour
{
    [SerializeField] private ARAnchorManager anchorManager;

    [SerializeField] private AnchorDataSO anchorData;
    [SerializeField] private AnchorDataSO moedelData;
    [SerializeField] private GameObject anchorPrefab;
    [SerializeField] private GameObject modelPrefab;

    //Debug
    [SerializeField] private Text anchorDebug;

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

        //UnityEngine.Quaternion quaternionModel = new UnityEngine.Quaternion(moedelData.quaternion.x, moedelData.quaternion.y, moedelData.quaternion.z, moedelData.quaternion.w);
        //ARGeospatialAnchor anchorModel = ARAnchorManagerExtensions.AddAnchor(anchorManager, moedelData.latitude, moedelData.longitude, moedelData.altitude, quaternion);

        Instantiate(anchorPrefab, anchor.transform);

        //GameObject model = Instantiate(modelPrefab, anchorModel.transform);
        //model.transform.Rotate(0, 180, -90);

        anchorDebug.text = "Anchor: Instantiate Finished!";

        //GeoInstantiated = true;
        //StartCoroutine(PlaceTerrainAnchor());        
    }

}
