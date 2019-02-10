using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScoreScreen : MonoBehaviour
{

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
        if (playerData.GameOver)
        {
            if (playerData.Score > playerData.OtherPlayerScore)
            {
                text.text = "Your Score: " + playerData.Score + "\n Opponents Score: " + playerData.OtherPlayerScore + "\n You Win!";
            }
            else
            {
                text.text = "Your Score: " + playerData.Score + "\n Opponents Score: " + playerData.OtherPlayerScore + "\n You Lose!";

            }
        }
        else {
            text.text = "";
        }
    }
}
