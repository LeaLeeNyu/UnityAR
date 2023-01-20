using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastableObject : MonoBehaviour
{
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private LevelInfoSO levelInfo;

    private void Awake()
    {
        levelInfo.unlocked = false;
    }

    public void HitRaycast()
    {
        infoPanel.SetActive(true);
        levelInfo.unlocked = true;
    }
}
