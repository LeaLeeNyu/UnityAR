using UnityEngine;

    public class MarkerClick : MonoBehaviour
    {
    public GameObject BridgeDetailPage;
    public GameObject ChristmasTreeDetailPage;
    public GameObject ParkDetailPage;
    private void Start()
        {
            OnlineMaps map = OnlineMaps.instance;

            // Add OnClick events to markers
            foreach (OnlineMapsMarker marker in OnlineMapsMarkerManager.instance)
            {
                marker.OnClick += OnMarkerClick;
            }

        }

        private void OnMarkerClick(OnlineMapsMarkerBase marker)
        {
        // Show detail page
        if (marker.label == "BrooklynBridge") {
            BridgeDetailPage.GetComponent<UIPanalMovement>().MoveUp();
        };

        if (marker.label == "ChristmasTree")
        {
            ChristmasTreeDetailPage.GetComponent<UIPanalMovement>().MoveUp();
        };
        if (marker.label == "Park")
        {
            ParkDetailPage.GetComponent<UIPanalMovement>().MoveUp();
        };
    }
    }
