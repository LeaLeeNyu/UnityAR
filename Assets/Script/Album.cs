using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Album : MonoBehaviour
{
    [SerializeField] private GameObject treeAfter;
    [SerializeField] private GameObject bridgeAfter;
    [SerializeField] private GameObject parkAfter;
    [SerializeField] private GameObject finalAfter;
    [SerializeField] private LevelInfoSO treeInfo;
    [SerializeField] private LevelInfoSO bridgeInfo;
    [SerializeField] private LevelInfoSO parkInfo;

    private void OnEnable()
    {
        if(treeInfo.unlocked)
        {
            treeAfter.SetActive(true);
        }
        else
        {
            treeAfter.SetActive(false);
        }

        if (bridgeInfo.unlocked)
        {
            bridgeAfter.SetActive(true);
        }
        else
        {
            treeAfter.SetActive(false);
        }

        if (parkInfo.unlocked)
        {
            parkAfter.SetActive(true);
        }
        else
        {
            parkAfter.SetActive(false);
        }

        if(treeInfo.unlocked && bridgeInfo.unlocked && parkInfo.unlocked)
        {
            finalAfter.SetActive(true);
        }
        else
        {
            finalAfter.SetActive(false);
        }
    }

}
