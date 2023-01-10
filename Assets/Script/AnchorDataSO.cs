using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="AnchorDataSO",menuName ="ScriptableObject / ARAnchorSO")]
public class AnchorDataSO : ScriptableObject
{
    public double latitude;
    public double longitude;
    public double altitude;
    public Vector4 quaternion;
}
