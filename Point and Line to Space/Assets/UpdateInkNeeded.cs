using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInkNeeded : MonoBehaviour {

    public PlayerData playerData;
    private Image bar;

    void Start () {
        bar = GetComponent<Image>();

    }

    // Update is called once per frame
    void Update () {
        float ink = playerData.Ink / 100;

        float inkNeeded = playerData.InkNeededForCurrentLine / 100;

        if (ink - inkNeeded < 0)
        {
            bar.color = new Color(1, 0, 0);
        }
        else {
            bar.color = new Color(1, 1, 0);
        }

        var inkBottom = ink - inkNeeded;
        if (inkBottom < 0) {
            inkBottom = 0;
        }

        var inkTop = ink;

        bar.rectTransform.anchorMax = new Vector2(bar.rectTransform.anchorMax.x, ink);
        bar.rectTransform.anchorMin = new Vector2(bar.rectTransform.anchorMin.x, inkBottom);
        


    }
}
