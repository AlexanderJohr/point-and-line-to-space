using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Line : NetworkBehaviour
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
    //private GameObject pointSphere1;
    //private GameObject pointSphere2;
    //private GameObject lineCylinder;

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


    public Vector3 EndPoint
    {
        get
        {
            return _endPoint;
        }
        set
        {
            _endPoint = value;

            if (linePlane != null)//if (pointSphere2 != null && lineCylinder != null)
            {
                //pointSphere2.transform.position = _endPoint;

                Vector3 lineDir = _endPoint - _startPoint;
                float halfDirMagnitude = lineDir.magnitude / 2;

                Vector3 centerBetweenStartAndEnd = _startPoint + (lineDir.normalized * halfDirMagnitude);
                //lineCylinder.transform.position = centerBetweenStartAndEnd;
                linePlane.transform.position = centerBetweenStartAndEnd;
                //Vector3 lineDirUpVector = new Vector3(lineDir.x, lineDir.y, lineDir.z + 90);

                //Quaternion dirRotation = Quaternion.LookRotation(lineDir);
                //Quaternion dirRotationPlus90 = Quaternion.LookRotation(lineDir);
                //Quaternion dirRotationZ90 = Quaternion.LookRotation(lineDir);


                //Vector3 dirRotationEuler = dirRotationPlus90.eulerAngles;
                //dirRotationEuler.x += 90;
                //dirRotationPlus90 = Quaternion.Euler(dirRotationEuler);

                //Vector3 dirRotationEulerZ90 = dirRotationZ90.eulerAngles;
                //dirRotationEulerZ90.z = 90;
                //dirRotationZ90 = Quaternion.Euler(dirRotationEulerZ90);

                //lineCylinder.transform.rotation = dirRotationPlus90;
                // linePlane.transform.rotation = dirRotationZ90;


                //Vector3 cylinderLocalScale = lineCylinder.transform.localScale;
                //cylinderLocalScale.y = lineDir.magnitude / 2;
                //lineCylinder.transform.localScale = cylinderLocalScale;

                Vector3 planeLocalScale = linePlane.transform.localScale;
                planeLocalScale.z = lineDir.magnitude / 2 / 5;
                linePlane.transform.localScale = planeLocalScale;

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

        //pointSphere1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //pointSphere1.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        //pointSphere2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //pointSphere2.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
        //lineCylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        //lineCylinder.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);

        linePlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        linePlane.transform.localScale = new Vector3(0.04f, 1f, 1f);
        linePlane.GetComponent<Renderer>().material = preReleaseMaterial;

        if (_playerId == 1)
        {
            linePlane.GetComponent<Renderer>().material = playerOneMaterial;
            //pointSphere1.GetComponent<Renderer>().material = playerOneMaterial;
            //pointSphere2.GetComponent<Renderer>().material = playerOneMaterial;
            //lineCylinder.GetComponent<Renderer>().material = playerOneMaterial;
        }
        else
        {
            linePlane.GetComponent<Renderer>().material = playerTwoMaterial;
            //pointSphere1.GetComponent<Renderer>().material = playerTwoMaterial;
            //pointSphere2.GetComponent<Renderer>().material = playerTwoMaterial;
            //lineCylinder.GetComponent<Renderer>().material = playerTwoMaterial;
        }

        _updateVisibilities();

    }

    // Update is called once per frame
    void Update()
    {
        if (camera != null && (_endPoint - _startPoint) != Vector3.zero)
        {
            Vector3 projectedPoint = Vector3.Project(camera.transform.position - _startPoint, _endPoint - _startPoint) + _startPoint;
            var lookRotation = Quaternion.LookRotation(camera.transform.position - projectedPoint, projectedPoint - _startPoint);
            //var gizmoQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            //gizmoQuad.transform.position = projectedPoint;
            //gizmoQuad.transform.rotation = lookRotation;
            //GameObject.Destroy(gizmoQuad, 1);

            var lookRotationPlane = Quaternion.LookRotation(_endPoint - _startPoint, camera.transform.position - projectedPoint);
            linePlane.transform.rotation = lookRotationPlane;

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
        //GameObject.Destroy(pointSphere1);
        //GameObject.Destroy(pointSphere2);
        //GameObject.Destroy(lineCylinder);
    }

    private bool _dead = false;
    private MaterialType _currentMaterial;

    public void Die()
    {
        linePlane.GetComponent<Renderer>().material = releaseImpossibleMaterial;
        _dead = true;
        GameObject.Destroy(this,1);
    }

    public int Id { get; set; }

    public bool IntersectsShape { get; set; }

    public bool InsufficientInk { get; set; }
}
