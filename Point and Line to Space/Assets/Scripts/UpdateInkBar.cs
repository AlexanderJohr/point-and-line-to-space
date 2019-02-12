using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInkBar : MonoBehaviour {

    public PlayerData playerData;
    private Image bar;

    void Start()
    {
        bar = GetComponent<Image>();
    }


    void Update()
    {
        float remainingInk = playerData.Ink - playerData.InkNeededForCurrentLine;
        Vector3 oldScale = bar.rectTransform.localScale;
        var inkBarHeight = Mathf.Max(remainingInk / 100, 0);

        bar.rectTransform.localScale = new Vector3(oldScale.x, inkBarHeight, oldScale.z);
    }
}
