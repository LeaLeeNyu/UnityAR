using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    Vector2 startPos;
    float movePos;
    [SerializeField] private float posThreshold;
    [SerializeField] private LevelInfoSO levelInfo;
    [SerializeField] private Animator noteAnimator;
    [SerializeField] private GameObject beforeNote;
    [SerializeField] private GameObject afterNote;
    [SerializeField] private GameObject beforeImg;
    [SerializeField] private GameObject afterImg;


    private void OnEnable()
    {
        if(levelInfo.unlocked)
        {
            beforeNote.SetActive(false);
            afterNote.SetActive(true);
            beforeImg.SetActive(false);
            afterImg.SetActive(true);
        }
        else
        {
            beforeNote.SetActive(true);
            afterNote.SetActive(false);
            beforeImg.SetActive(true);
            afterImg.SetActive(false);
        }
    }

    private void Update()
    {
        HandleSwipe();
    }


    /// <summary>
    /// Invokes TouchPositionChanged event the screen is swiped.
    /// Returned Vec2 is normalized screen position
    /// </summary>
    private void HandleSwipe()
    {
        if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            startPos = Input.touches[0].position;
            return;
        }


        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved)
        {
            movePos = Input.touches[0].position.y - startPos.y;

            if (movePos > posThreshold)
            {
                noteAnimator.SetBool("up", true);
            }

            if (movePos < -posThreshold)
            {
                noteAnimator.SetBool("up", false);
            }

        }
    }


}
