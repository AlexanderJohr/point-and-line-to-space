using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeAwayScoreText : MonoBehaviour {
    Text text;
    public float fadeAwaySpeed = 0.1f;

    public float floatToTopSpeed = 0.1f;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        var old = transform.position;
        transform.position = new Vector3(old.x, old.y + floatToTopSpeed * Time.deltaTime, old.z);
        var oldColor = text.color;
        text.color = new Color(oldColor.r, oldColor.g, oldColor.b, oldColor.a- fadeAwaySpeed * Time.deltaTime);

    }
}
