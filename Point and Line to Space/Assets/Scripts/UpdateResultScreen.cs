using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateResultScreen : MonoBehaviour
{

    public Text winLoseText;
    public Text scoreText;
    public Text opponentScoreText;

    public GameObject winFx;
    public GameObject loseFx;


    public PlayerData playerData;
    private Text text;
    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (playerData.OtherPlayerScore != -1)
        {
            if (playerData.GameOver)
            {
                if (playerData.Score > playerData.OtherPlayerScore)
                {
                    winFx.SetActive(true);
                    winLoseText.color = new Color(194, 0, 0);
                    winLoseText.text = "You Win!";

                    scoreText.color = new Color(194, 0, 0);
                    scoreText.text = playerData.Score.ToString("F0");

                    opponentScoreText.text = playerData.OtherPlayerScore.ToString("F0");

                }
                else
                {
                    loseFx.SetActive(true);

                    winLoseText.color = new Color(0, 0, 194);
                    winLoseText.text = "You Lose!";

                    scoreText.color = new Color(0, 0, 194);
                    scoreText.text = playerData.Score.ToString("F0");

                    opponentScoreText.text = playerData.OtherPlayerScore.ToString("F0");
                }
            }
        }
    }
}
