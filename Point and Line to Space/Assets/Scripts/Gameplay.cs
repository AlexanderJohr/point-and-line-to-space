using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameplay : MonoBehaviour
{
    public LineRenderer linePrefab;
    public MeshFilter shapePrefab;

    public String myName;

    public float drawDistanceToCamera;

    private Line currentLine;

    private List<Line> drawnLines = new List<Line>();
    private List<Shape> drawnShapes = new List<Shape>();

    List<Vector3> intersectionPoints = new List<Vector3>();

    public bool infiniteLength = true;
    Camera screenCamera;
    public AudioSource audioData;

    void Start()
    {
        print(myName);
        screenCamera = GetComponent<Camera>();
    }

    RaycastHit2D hit;
    Vector3[] touches = new Vector3[5];

    void Update()
    {
        if (Input.touchCount >= 2)
        {
            foreach (Touch t in Input.touches)
            {
                Vector2 position = Input.GetTouch(t.fingerId).position;
                touches[t.fingerId] = screenCamera.ScreenToWorldPoint(position) + transform.forward * drawDistanceToCamera;
            }

            Vector3 start = touches[0];
            Vector3 end = touches[1];

            if (currentLine == null)
            {
                currentLine = new Line(Instantiate<LineRenderer>(linePrefab));
            }

            if (infiniteLength)
            {
                Vector3 lineVector = start - end;
                Vector3 infiniteEnd = end + (lineVector * 1000);
                Vector3 infiniteStart = start - (lineVector * 1000);
                start = infiniteStart;
                end = infiniteEnd;
            }

            currentLine.Start = start;
            currentLine.End = end;
        }
        else
        {
            if (currentLine != null)
            {
                drawnLines.Add(currentLine);
                currentLine = null;
            }
        }

        intersectionPoints.Clear();
        List<Line> alreadyCheckedLines = new List<Line>();
        foreach (Line a in drawnLines)
        {
            foreach (Line b in drawnLines)
            {
                if (a != b && !alreadyCheckedLines.Contains(b)) {
                    

                    Vector3 a1 = screenCamera.WorldToScreenPoint(a.Start);
                    Vector3 a2 = screenCamera.WorldToScreenPoint(a.End);
                    Vector3 b1 = screenCamera.WorldToScreenPoint(b.Start);
                    Vector3 b2 = screenCamera.WorldToScreenPoint(b.End);

                    bool found;
                    Vector2 intersection = GetIntersectionPointCoordinates(a1, a2, b1, b2, out found);
                    if (found)
                    {                                                                                                   
                        Vector3 intersectionPoint = screenCamera.ScreenToWorldPoint(intersection)
                                                  // never set inside this code
                            + transform.forward * drawDistanceToCamera;
                        intersectionPoints.Add(intersectionPoint);
                    }
                }
            }
            alreadyCheckedLines.Add(a);
        }
        
        if (intersectionPoints.Count == 3) {
            Shape shape = new Shape(Instantiate<MeshFilter>(shapePrefab));
            shape.Vertices = intersectionPoints;
            drawnShapes.Add(shape);

            audioData.Play(0);

            drawnLines.ForEach((line) => Destroy(line.Renderer));
            drawnLines.Clear();
        }
    }
    Vector3 gizmoPos = new Vector3();
    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        foreach (Vector3 intersectionPoint in intersectionPoints)
        {            
            Gizmos.DrawSphere(intersectionPoint, 0.5f);
        }
    }

    /// <summary>
    /// Gets the coordinates of the intersection point of two lines.
    /// </summary>
    /// <param name="A1">A point on the first line.</param>
    /// <param name="A2">Another point on the first line.</param>
    /// <param name="B1">A point on the second line.</param>
    /// <param name="B2">Another point on the second line.</param>
    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
    private Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
    {
        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

        if (tmp == 0)
        {
            // No solution!
            found = false;
            return Vector2.zero;
        }

        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

        found = true;

        return new Vector2(
            B1.x + (B2.x - B1.x) * mu,
            B1.y + (B2.y - B1.y) * mu
        );
    }
}


class Line
{
    private Vector3 _start;
    public Vector3 Start
    {
        get
        {
            return _start;
        }
        set
        {
            _start = value;
            Renderer.SetPosition(0, _start);
        }
    }

    private Vector3 _end;

    public Vector3 End
    {
        get
        {
            return _end;
        }
        set
        {
            _end = value;
            Renderer.SetPosition(1, _end);
        }
    }
    public LineRenderer Renderer { get; private set; }

    public Line(LineRenderer renderer)
    {
        this.Renderer = renderer;
    }
}

class Shape
{
    private List<Vector3> _vertices;

    public List<Vector3> Vertices
    {
        get
        {
            return _vertices;
        }
        set
        {
            _vertices = value;
            MeshFilter.mesh.vertices = _vertices.ToArray();

            int[] tri = new int[3];

            tri[0] = 0;
            tri[1] = 1;
            tri[2] = 2;

            MeshFilter.mesh.triangles = tri;

            Vector3[] normals = new Vector3[3];

            normals[0] = Vector3.forward;
            normals[1] = Vector3.forward;
            normals[2] = Vector3.forward;

            MeshFilter.mesh.normals = normals;


        }
    }
    
    public MeshFilter MeshFilter { get; private set; }

    public Shape(MeshFilter meshFilter)
    {
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;

        this.MeshFilter = meshFilter;

    }
}

