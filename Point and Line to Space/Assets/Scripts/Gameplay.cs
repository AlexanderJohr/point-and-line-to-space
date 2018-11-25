using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    List<Intersection> intersections = new List<Intersection>();

    public bool infiniteLength = true;
    Camera screenCamera;

    public AudioSource audioData;

    public AudioSource exampleSound;


    public int desiredIntersectionPointCount = 3;

    void Start()
    {
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

        intersections.Clear();
        List<Line> alreadyCheckedLines = new List<Line>();
        foreach (Line a in drawnLines)
        {
            foreach (Line b in drawnLines)
            {
                if (a != b && !alreadyCheckedLines.Contains(b))
                {


                    Vector2 a1 = screenCamera.WorldToScreenPoint(a.Start);
                    Vector2 a2 = screenCamera.WorldToScreenPoint(a.End);
                    Vector2 b1 = screenCamera.WorldToScreenPoint(b.Start);
                    Vector2 b2 = screenCamera.WorldToScreenPoint(b.End);

                    bool found;
                    // I get all the intersection points here:
                    Vector2 intersectionPoint = GetIntersectionPointCoordinates(a1, a2, b1, b2, out found);
                    bool isInLineSegmentA = CheckPointIsInLineSegment(a1, a2, intersectionPoint);
                    bool isInLineSegmentB = CheckPointIsInLineSegment(b1, b2, intersectionPoint);

                    // I get all intersection points that are visible on screen
                    bool isInsideScreen = screenCamera.pixelRect.Contains(intersectionPoint);
                    if (found && isInsideScreen && isInLineSegmentA && isInLineSegmentB)
                    {

                        Intersection intersection = new Intersection(intersectionPoint, a, b);
                        a.Intersections.Add(intersection);
                        b.Intersections.Add(intersection);
                        intersections.Add(intersection);
                    }
                }
            }
            alreadyCheckedLines.Add(a);
        }

        List<Vector2> maxVertexList = new List<Vector2>();
        foreach (Intersection intersection in intersections)
        {
            Intersection firstIntersection = intersection;
            Intersection nextIntersection = intersection;

            List<Vector2> vertexList = new List<Vector2>();

            do
            {
                vertexList.Add(nextIntersection.Vertex);

                Line edge1 = intersection.Edge1;
                Line edge2 = intersection.Edge2;

                nextIntersection = intersections.Where(i => i != nextIntersection && i.Edge2 == edge1 || i.Edge1 == edge1 || i.Edge2 == edge2 || i.Edge1 == edge2).First();

            } while (nextIntersection != null || nextIntersection != firstIntersection);
            bool gotCycle = nextIntersection == firstIntersection;

            if (gotCycle && vertexList.Count >= 3 && vertexList.Count > maxVertexList.Count)
            {
                maxVertexList = vertexList;
            }
        }

        if (maxVertexList.Count >= 3)
        {
            List<Vector3> convexHull3D = maxVertexList.Select(v => screenCamera.ScreenToWorldPoint(v) + transform.forward * drawDistanceToCamera).ToList();

            Shape shape = new Shape(Instantiate<MeshFilter>(shapePrefab), convexHull3D);

            drawnShapes.Add(shape);

            audioData.Play(0);

            drawnLines.ForEach((line) => Destroy(line.Renderer));
            drawnLines.Clear();
        }
    }

    private bool CheckPointIsInLineSegment(Vector2 a1, Vector2 a2, Vector2 intersection)
    {
        return a1.x < intersection.x && a2.x > intersection.x || a2.x < intersection.x && a1.x > intersection.x &&
               a1.y < intersection.y && a2.y > intersection.y || a2.y < intersection.y && a1.y > intersection.y;
    }



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

    class Shape
    {

        public MeshFilter MeshFilter { get; private set; }

        public Shape(MeshFilter meshFilter, List<Vector3> convexHull)
        {
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;

            meshFilter.mesh.vertices = convexHull.ToArray();

            List<Triangle> triangles = JarvisMarchAlgorithm.TriangulateConvexPolygon(convexHull);
            int[] tri = new int[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {

                tri[i * 3] = 0;
                tri[(i * 3) + 1] = i + 1;
                tri[(i * 3) + 2] = i + 2;



            }


            Vector3[] normals = new Vector3[convexHull.Count];
            for (int i = 0; i < normals.Count(); i++)
            {
                normals[i] = Vector3.forward;
            }


            meshFilter.mesh.normals = normals;



            meshFilter.mesh.triangles = tri;


            this.MeshFilter = meshFilter;

        }

    }
}


class Line
{

    public List<Intersection> Intersections { get; set; }

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

    public Vector3 Direction
    {
        get
        {
            return _end - _start;
        }
    }




    public LineRenderer Renderer { get; private set; }

    public Line(LineRenderer renderer)
    {
        this.Renderer = renderer;
        Intersections = new List<Intersection>();
    }
}



public static class JarvisMarchAlgorithm
{

    public static List<Triangle> TriangulateConvexPolygon(List<Vector3> convexHullpoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < convexHullpoints.Count; i++)
        {
            Vector3 a = convexHullpoints[0];
            Vector3 b = convexHullpoints[i - 1];
            Vector3 c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }


}

public class Triangle
{
    //Corners
    public Vector3 v1;
    public Vector3 v2;
    public Vector3 v3;

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }
}

class Intersection
{
    public Vector2 Vertex { get; private set; }
    public Line Edge1 { get; private set; }
    public Line Edge2 { get; private set; }

    public Intersection(Vector2 vertex, Line edge1, Line edge2)
    {
        Vertex = vertex;
        Edge1 = edge1;
        Edge2 = edge2;
    }
}


