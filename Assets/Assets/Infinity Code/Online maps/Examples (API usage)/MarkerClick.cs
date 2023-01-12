using UnityEngine;

    public class MarkerClick : MonoBehaviour
    {
    public GameObject BridgeDetailPage;
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
            BridgeDetailPage.SetActive(true);
        };
       
        }
    }
