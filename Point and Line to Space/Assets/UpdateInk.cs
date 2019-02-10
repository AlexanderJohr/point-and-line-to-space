using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInk : MonoBehaviour {

    public PlayerData playerData;
    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }


    void Update()
    {
        float remainingInk = playerData.Ink - playerData.InkNeededForCurrentLine;

        text.text = Mathf.Round(remainingInk).ToString();
    }
}
