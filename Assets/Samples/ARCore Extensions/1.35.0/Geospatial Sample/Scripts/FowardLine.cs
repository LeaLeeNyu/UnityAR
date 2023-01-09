using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FowardLine : MonoBehaviour
{
    // Start is called before the first frame update
    LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount=2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.forward * 20 + transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
