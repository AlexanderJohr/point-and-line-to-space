using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScore : MonoBehaviour
{

    public PlayerData playerData;
    private Text text;
    private float scoreTextValue = 0;

    public float scoreStep = 1;

    public float ScoreTextValue
    {
        get
        {
            return scoreTextValue;
        }

        set
        {
            scoreTextValue = value;
            text.text = scoreTextValue.ToString("F0"); ;
        }
    }

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

        if (playerData.Score > ScoreTextValue + scoreStep)
        {
            ScoreTextValue += scoreStep;
        }
        else if (playerData.Score != ScoreTextValue)
        {
            ScoreTextValue = playerData.Score;
        }
    }
}
