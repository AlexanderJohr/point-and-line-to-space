using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTriger: MonoBehaviour {
    // Update is called once per frame
    void Update () {
        
        if ((Input.touchCount > 0))
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                print("Do");
                GameObject.Find("Canvas").transform.Find("Menu").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Title").gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("TapScreenToStart").gameObject.SetActive(false);
            }
        }
    }
}
