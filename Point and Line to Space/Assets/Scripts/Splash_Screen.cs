using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splash_Screen : MonoBehaviour {

    public float timeover;
    private float timeElapsed;

	void Update () {
        timeElapsed += Time.deltaTime;
        if(timeElapsed>=timeover){
            SceneManager.LoadScene("Menu Screen");
        }
	}
}
