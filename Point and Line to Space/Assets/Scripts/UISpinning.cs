using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISpinning : MonoBehaviour{
    // Update is called once per frame
    public float angVelocity;
    void Start()
    {

    }
    void Update()
    {
        transform.Rotate(Vector3.right, angVelocity * Time.deltaTime);
    }
}