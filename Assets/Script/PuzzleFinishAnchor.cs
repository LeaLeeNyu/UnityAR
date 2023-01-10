using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleFinishAnchor : MonoBehaviour
{
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

    }
}
