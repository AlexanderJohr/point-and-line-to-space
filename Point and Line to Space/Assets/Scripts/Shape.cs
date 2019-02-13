using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    private Vector3[] _convexHull;

    public MeshFilter MeshFilter { get { return GetComponent<MeshFilter>(); } }

    public Material playerOneMaterial;
    public Material playerTwoMaterial;


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

    public int Id { get; internal set; }

    public Vector3[] ConvexHull
    {
        get
        {
            return _convexHull;
        }
        set
        {
            _convexHull = value;

            Mesh mesh = new Mesh();

            mesh.vertices = _convexHull;

            List<UpdateLocalPlayer.Triangle> triangles = UpdateLocalPlayer.TriangulateConvexPolygon(_convexHull);
            int[] tri = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {

                tri[i * 3] = 0;
                tri[(i * 3) + 1] = i + 1;
                tri[(i * 3) + 2] = i + 2;
            }


            Vector3[] normals = new Vector3[_convexHull.Length];
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i] = Vector3.forward;
            }

            mesh.normals = normals;
            mesh.triangles = tri;

            MeshFilter.mesh = mesh;
        }
    }

    

    // Use this for initialization
    void Start()
    {
        if (_playerId == 1)
        {
            GetComponent<Renderer>().material = playerOneMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = playerTwoMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
