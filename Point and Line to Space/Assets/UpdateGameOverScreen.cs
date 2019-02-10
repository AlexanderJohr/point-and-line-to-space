using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateGameOverScreen : MonoBehaviour
{

    public PlayerData playerData;

    void Update()
    {
        if (playerData.GameOver)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
    }
}
