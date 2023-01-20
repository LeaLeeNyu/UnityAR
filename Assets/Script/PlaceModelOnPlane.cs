using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
public class PlaceModelOnPlane : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    //private GameObject[] animals;
    public GameObject placedCube;


    public bool useCursor = true;


    void Start()
    {
        // animals = Resources.LoadAll<GameObject>("Animals");

    }

    void Update()
    {
        if (useCursor)
        {
            UpdateCursor();
        }

        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && useCursor)
        //{
        //    // int animalId = StaticData.catchingAnimalId;
        //    // placedAnimal = Instantiate(animals[animalId], transform.position, transform.rotation);
        //    // placedAnimal.transform.localScale = new Vector3(4f, 4f, 4f);

        //    Instantiate(placedCube, transform.position, transform.rotation);

        //    Vector3 cameraPostition = new Vector3(Camera.main.transform.position.x,
        //                                transform.position.y,
        //                                Camera.main.transform.position.z);
        //    placedCube.transform.LookAt(cameraPostition);

        //    useCursor = false;

        //}


    }

    void UpdateCursor()
    {
        Vector2 screenPos = new Vector2(Screen.width / 2, Screen.height / 2);
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        raycastManager.Raycast(screenPos, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;

        }
    }

    public void PlaceModel()
    {
        if (useCursor) {
            Instantiate(placedCube, transform.position, transform.rotation);

            Vector3 cameraPostition = new Vector3(Camera.main.transform.position.x,
                                        transform.position.y,
                                        Camera.main.transform.position.z);
            placedCube.transform.LookAt(cameraPostition);

            useCursor = false;
        }
       
    }
}

