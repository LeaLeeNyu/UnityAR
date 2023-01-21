using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GeospatialManager geospatial;
    private GameObject cubeAnchor;


    private void Update()
    {
        slider.value = geospatial.dotParameter;
    }
}
