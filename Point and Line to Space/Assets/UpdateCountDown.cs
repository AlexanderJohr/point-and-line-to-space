using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateCountDown : MonoBehaviour {

    bool timeLabelAlreadyUpdated = false;

    public PlayerData playerData;
    private Text text;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = Mathf.Ceil(playerData.RemainingSeconds).ToString();
        if (!timeLabelAlreadyUpdated) {
            if (playerData.RemainingSeconds < 20)
            {
                text.fontSize = (int)(text.fontSize * 1.5);
                text.color = Color.red;
                timeLabelAlreadyUpdated = true;
            }
        }
    }
}
