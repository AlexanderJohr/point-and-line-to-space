using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Line : NetworkBehaviour
{
    public List<Intersection> Intersections { get; set; }


    private Vector3 _endPoint;

    private Vector3 _startPoint;

    public Vector3 StartPoint
    {
        get
        {
            return _startPoint;
        }
        set
        {
            _startPoint = value;

            GetComponent<LineRenderer>().SetPosition(0, _startPoint);

        }
    }
 

    public Vector3 EndPoint
    {
        get
        {
            return _endPoint;
        }
        set
        {
            _endPoint = value;
            GetComponent<LineRenderer>().SetPosition(1, _endPoint);

        }
    }


    public Vector3 Direction
    {
        get
        {
            return _endPoint - _startPoint;
        }
    }




    public LineRenderer Renderer { get; private set; }







    private LineRenderer _lineRenderer;
    // Use this for initialization
    void Start () {
        Intersections = new List<Intersection>();

        _lineRenderer = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
