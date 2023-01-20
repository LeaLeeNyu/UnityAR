using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectObject : MonoBehaviour
{
    private Vector2 _touchPoint;
    public Vector2 NormalizedTouchPoint { get => NormalizedScreenPosition(_touchPoint); }

    private GameObject _touchedObject;

    [SerializeField] private float _screenTapLowerCutoff = 0.15f;
    [SerializeField] private CheckPosManager checkPosManager;
    [SerializeField] private Camera ARCamera;
    [SerializeField] private string triggerName;

    public Text inputDebug;

    public Vector2 NormalizedScreenPosition(Vector2 screenPos)
    {
        float x = Remap(screenPos.x, 0, Screen.width, 0, 1);
        float y = Remap(screenPos.y, 0, Screen.height, 0, 1);

        return new Vector2(x, y);
    }

    private void Update()
    {
        //if (checkPosManager.finishPuzzle)
        //{
            HandleScreenTap();
        //}
    }

    private void HandleScreenTap()
    {
        

        // if player tape the screen
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            _touchPoint = Input.touches[0].position;
        }

        else return;

        if (NormalizedTouchPoint.y < _screenTapLowerCutoff)
            return;

        inputDebug.text = Input.touches[0].position.ToString();

        Ray ray = ARCamera.ScreenPointToRay(_touchPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            _touchedObject = hit.collider.gameObject;
            inputDebug.text = hit.collider.gameObject.tag;

            if (_touchedObject.name == triggerName)
            {                
                RaycastableObject raycastable = _touchedObject.GetComponent<RaycastableObject>();
                raycastable.HitRaycast();
                inputDebug.text = "Hit!";
            }
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float fromAbs = value - fromMin;
        float fromMaxAbs = fromMax - fromMin;

        float normal = fromAbs / fromMaxAbs;

        float toMaxAbs = toMax - toMin;
        float toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }


}
