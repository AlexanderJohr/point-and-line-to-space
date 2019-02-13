using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Line : MonoBehaviour
{
    public List<Intersection> Intersections { get; set; }
    public bool IsReleased
    {
        get
        {
            return _isReleased;
        }
        set
        {
            _isReleased = value;
            _updateVisibilities();
        }
    }

    private void _updateVisibilities()
    {
        if (_isReleased)
        {
            //if (pointSphere1 != null) pointSphere1.SetActive(true);
            //if (pointSphere2 != null) pointSphere2.SetActive(true);
            //if (lineCylinder != null) lineCylinder.SetActive(true);
            if (linePlane != null) linePlane.SetActive(true);
        }
        else
        {
            //if (pointSphere1 != null) pointSphere1.SetActive(false);
            //if (pointSphere2 != null) pointSphere2.SetActive(false);
            //if (lineCylinder != null) lineCylinder.SetActive(false);
            if (linePlane != null) linePlane.SetActive(true);
        }
    }

    private Camera camera;

    private GameObject linePlane;

    public Material playerOneMaterial;
    public Material playerTwoMaterial;
    public Material preReleaseMaterial;
    public Material releaseImpossibleMaterial;

    private int _playerId;

    public int PlayerId
    {
        get
        {
            return _playerId;
        }
        set
        {
            _playerId = value;
        }
    }



    public bool IsLocalLine
    {
        get
        {
            return _isLocalLine;
        }
        set
        {
            _isLocalLine = value;
        }
    }

    public float Magnitude
    {
        get
        {
            return (_startPoint - _endPoint).magnitude;
        }
    }

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
        }
    }

    private bool _endPointChanged = false;
    public Vector3 EndPoint
    {
        get
        {
            return _endPoint;
        }
        set
        {
            if (_endPoint != value)
            {
                _endPoint = value;
                _endPointChanged = true;
            }


        }
    }


    public Vector3 Direction
    {
        get
        {
            return _endPoint - _startPoint;
        }
    }

    private bool _isLocalLine = false;
    private bool _isReleased;
    private bool _impossibleToRelease = false;

    // Use this for initialization
    void Start()
    {
        Intersections = new List<Intersection>();

        linePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        linePlane.transform.localScale = new Vector3(0.04f, 1f, 1f);
        linePlane.GetComponent<Renderer>().material = preReleaseMaterial;

        if (_playerId == 1)
        {
            linePlane.GetComponent<Renderer>().material = playerOneMaterial;
        }
        else
        {
            linePlane.GetComponent<Renderer>().material = playerTwoMaterial;
        }

        _updateVisibilities();

    }

    // Update is called once per frame
    void Update()
    {
        if (camera != null)
        {
            if ((_endPoint - _startPoint) != Vector3.zero)
            {
                if (_endPointChanged)
                {
                    Vector3 projectedPoint = Vector3.Project(camera.transform.position - _startPoint, _endPoint - _startPoint) + _startPoint;
                    var lookRotation = Quaternion.LookRotation(camera.transform.position - projectedPoint, projectedPoint - _startPoint);

                    var lookRotationPlane = Quaternion.LookRotation(_endPoint - _startPoint, camera.transform.position - projectedPoint);
                    linePlane.transform.rotation = lookRotationPlane;


                    Vector3 lineDir = _endPoint - _startPoint;
                    float halfDirMagnitude = lineDir.magnitude / 2;

                    Vector3 centerBetweenStartAndEnd = _startPoint + (lineDir.normalized * halfDirMagnitude);
                    linePlane.transform.position = centerBetweenStartAndEnd;

                    Vector3 planeLocalScale = linePlane.transform.localScale;
                    planeLocalScale.z = lineDir.magnitude / 2 / 5;
                    linePlane.transform.localScale = planeLocalScale;

                    _endPointChanged = false;
                }
            }


        }
        else
        {
            camera = Camera.current;
        }

        if (_dead)
        {
            Color color = linePlane.GetComponent<Renderer>().material.color;
            color.a = color.a - 0.01f;
            linePlane.GetComponent<Renderer>().material.color = color;
        }

        if (InsufficientInk || IntersectsShape)
        {
            CurrentMaterial = MaterialType.Interupted;
        }
        else
        {
            CurrentMaterial = MaterialType.Normal;
        }
    }
    public enum MaterialType
    {
        Normal,
        Interupted
    }

    public MaterialType CurrentMaterial
    {
        get
        {
            return _currentMaterial;
        }
        set
        {
            if (value != _currentMaterial)
            {
                _currentMaterial = value;
                if (CurrentMaterial == MaterialType.Normal)
                {
                    linePlane.GetComponent<Renderer>().material = playerOneMaterial;
                }
                else
                {
                    linePlane.GetComponent<Renderer>().material = releaseImpossibleMaterial;
                }
            }
        }
    }

    private void OnDestroy()
    {
        GameObject.Destroy(linePlane);
    }

    private bool _dead = false;
    private MaterialType _currentMaterial;

    public void Die()
    {
        linePlane.GetComponent<Renderer>().material = releaseImpossibleMaterial;
        _dead = true;
        GameObject.Destroy(this, 1);
    }

    public int Id { get; set; }

    public bool IntersectsShape { get; set; }

    public bool InsufficientInk { get; set; }
}
