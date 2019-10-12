using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateLowInkNotifier : MonoBehaviour
{

    public PlayerData playerData;
    public GameObject NoInkPanel;

    [Range(0,1)]
    public float warningWhenInkLowerThen = 0.25f;

    void Start()
    {
        NoInkPanel.SetActive(false);
    }

    void Update()
    {
        float ink = playerData.Ink / 100;

        float inkNeeded = playerData.InkNeededForCurrentLine / 100;

        var remainingInk = ink - inkNeeded;

        var inkBottom = ink - inkNeeded;
        if (remainingInk > 0 && remainingInk < warningWhenInkLowerThen)
        {
            if (NoInkPanel.activeSelf != true)
            {
                NoInkPanel.SetActive(true);
            }
        }
        else
        {
            if (NoInkPanel.activeSelf != false)
            {
                NoInkPanel.SetActive(false);
            }
        }


    }
}
