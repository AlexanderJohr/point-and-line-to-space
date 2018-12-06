using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class Gameplay : NetworkBehaviour
{
    public Line linePrefab;
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
    Vector3?[] touches = new Vector3?[2];



    [SyncVar(hook = "OnEndChanged")]
    public Vector3 start;

    [SyncVar(hook = "OnStartChanged")]
    public Vector3 end;

    void OnStartChanged(Vector3 n)
    {
        print(String.Format("OnStartChanged {0}", start));

    }

    void OnEndChanged(Vector3 n)
    {
        print(String.Format("OnEndChanged {0}", end));

    }

    private List<Intersection> remainingIntersectionsToCheck;

    public Line CurrentLine
    {
        get
        {
            return currentLine;
        }

        set
        {
            currentLine = value;
        }
    }

    [Command]
    void CmdSetStart(Vector3 newStart)
    {
        start = newStart;
                if(CurrentLine!= null)
        CurrentLine.StartPoint = start;

    }
    [Command]
    void CmdSetEnd(Vector3 newEnd)
    {
        end = newEnd;
        if(CurrentLine!= null)
            CurrentLine.EndPoint = end;
    }

    [Command]
    void CmdCreateLine(Vector3 start, Vector3 end)
    {
        print(String.Format("Server start: {0}  end: {1}", start, end));
            CurrentLine = Instantiate(linePrefab);

        NetworkServer.Spawn(CurrentLine.gameObject);
    }

    void Update()
    {

        if (!isLocalPlayer) return;
        if (Input.touchCount >= 2)
        {
            foreach (Touch t in Input.touches)
            {
                Vector2 position = Input.GetTouch(t.fingerId).position;
                touches[t.fingerId] = screenCamera.ScreenToWorldPoint(position) + transform.forward * drawDistanceToCamera;
            }
        }
        else
        {
            if (touches[0] != null) {
                CmdSetStart (touches[0].Value);
            }
            if (touches[1] != null)
            {
                CmdSetEnd(touches[1].Value);
            }

            //  touches[0] = null;
            // touches[1] = null;
        }


        if (Input.GetMouseButton(0))
        {
            if (touches[0] == null)
            {
                touches[0] = screenCamera.ScreenToWorldPoint(Input.mousePosition) + transform.forward * drawDistanceToCamera;
            }
            else
            {
                touches[1] = screenCamera.ScreenToWorldPoint(Input.mousePosition) + transform.forward * drawDistanceToCamera;
            }
        }
        else
        {
            if (touches[0] != null)
            {
                CmdSetStart(touches[0].Value);
            }
            if (touches[1] != null)
            {
                CmdSetEnd(touches[1].Value);
            }

            touches[0] = null;
            touches[1] = null;
        }




        if (start != Vector3.zero && end != Vector3.zero) 
        {

            if (CurrentLine == null && start != Vector3.zero && end != Vector3.zero)
            {
                
                CmdCreateLine(start, end);

            }




            if (touches[0] == null && touches[1] == null && CurrentLine != null) {

                drawnLines.Add(CurrentLine);
                CurrentLine = null;
                CmdSetStart(Vector3.zero);
                CmdSetEnd(Vector3.zero);

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


                    Vector2 a1 = screenCamera.WorldToScreenPoint(a.StartPoint);
                    Vector2 a2 = screenCamera.WorldToScreenPoint(a.EndPoint);
                    Vector2 b1 = screenCamera.WorldToScreenPoint(b.StartPoint);
                    Vector2 b2 = screenCamera.WorldToScreenPoint(b.EndPoint);

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
            List<Intersection> remainingIntersectionsToCheck = intersections.Select(i => i).ToList();

            Intersection firstIntersection = intersection;
            Intersection nextIntersection = intersection;

            List<Vector2> vertexList = new List<Vector2>();

            do
            {
                vertexList.Add(nextIntersection.Vertex);

                Line edge1 = nextIntersection.Edge1;
                Line edge2 = nextIntersection.Edge2;

                var nextIntersections = remainingIntersectionsToCheck.Where(i => i != nextIntersection && (i.Edge2 == edge1 || i.Edge1 == edge1 || i.Edge2 == edge2 || i.Edge1 == edge2));

                if (nextIntersections.Count() > 0)
                {
                    nextIntersection = nextIntersections.First();

                    print(String.Format("remainingIntersections: {0}", remainingIntersectionsToCheck.Count));

                    remainingIntersectionsToCheck.Remove(nextIntersection);
                }
                else
                {
                    nextIntersection = null;
                }

                if (nextIntersection == firstIntersection)
                {
                    break;

                }


            } while (nextIntersection != null);
            bool gotCycle = nextIntersection == firstIntersection;

            if (gotCycle && vertexList.Count >= 3 && vertexList.Count > maxVertexList.Count)
            {
                maxVertexList = vertexList;
            }
        }
        if (maxVertexList.Count >= 3)
        {
            print(String.Format("maxVertexList: {0}", maxVertexList.Count()));

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

    public static List<Triangle> TriangulateConcavePolygon(List<Vector3> points)
    {
        //The list with triangles the method returns
        List<Triangle> triangles = new List<Triangle>();

        //If we just have three points, then we dont have to do all calculations
        if (points.Count == 3)
        {
            triangles.Add(new Triangle(points[0], points[1], points[2]));

            return triangles;
        }



        //Step 1. Store the vertices in a list and we also need to know the next and prev vertex
        List<Vertex> vertices = new List<Vertex>();

        for (int i = 0; i < points.Count; i++)
        {
            vertices.Add(new Vertex(points[i]));
        }

        //Find the next and previous vertex
        for (int i = 0; i < vertices.Count; i++)
        {
            int nextPos = ClampListIndex(i + 1, vertices.Count);

            int prevPos = ClampListIndex(i - 1, vertices.Count);

            vertices[i].prevVertex = vertices[prevPos];

            vertices[i].nextVertex = vertices[nextPos];
        }



        //Step 2. Find the reflex (concave) and convex vertices, and ear vertices
        for (int i = 0; i < vertices.Count; i++)
        {
            CheckIfReflexOrConvex(vertices[i]);
        }

        //Have to find the ears after we have found if the vertex is reflex or convex
        List<Vertex> earVertices = new List<Vertex>();

        for (int i = 0; i < vertices.Count; i++)
        {
            IsVertexEar(vertices[i], vertices, earVertices);
        }



        //Step 3. Triangulate!
        while (true)
        {
            //This means we have just one triangle left
            if (vertices.Count == 3)
            {
                //The final triangle
                triangles.Add(new Triangle(vertices[0], vertices[1], vertices[2]));

                break;
            }

            //Make a triangle of the first ear
            Vertex earVertex = earVertices[0];

            Vertex earVertexPrev = earVertex.prevVertex;
            Vertex earVertexNext = earVertex.nextVertex;

            Triangle newTriangle = new Triangle(earVertex, earVertexPrev, earVertexNext);

            triangles.Add(newTriangle);

            //Remove the vertex from the lists
            earVertices.Remove(earVertex);

            vertices.Remove(earVertex);

            //Update the previous vertex and next vertex
            earVertexPrev.nextVertex = earVertexNext;
            earVertexNext.prevVertex = earVertexPrev;

            //...see if we have found a new ear by investigating the two vertices that was part of the ear
            CheckIfReflexOrConvex(earVertexPrev);
            CheckIfReflexOrConvex(earVertexNext);

            earVertices.Remove(earVertexPrev);
            earVertices.Remove(earVertexNext);

            IsVertexEar(earVertexPrev, vertices, earVertices);
            IsVertexEar(earVertexNext, vertices, earVertices);
        }

        //Debug.Log(triangles.Count);

        return triangles;
    }



    //Check if a vertex if reflex or convex, and add to appropriate list
    private static void CheckIfReflexOrConvex(Vertex v)
    {
        v.isReflex = false;
        v.isConvex = false;

        //This is a reflex vertex if its triangle is oriented clockwise
        Vector2 a = v.prevVertex.GetPos2D_XZ();
        Vector2 b = v.GetPos2D_XZ();
        Vector2 c = v.nextVertex.GetPos2D_XZ();

        if (IsTriangleOrientedClockwise(a, b, c))
        {
            v.isReflex = true;
        }
        else
        {
            v.isConvex = true;
        }
    }



    //Check if a vertex is an ear
    private static void IsVertexEar(Vertex v, List<Vertex> vertices, List<Vertex> earVertices)
    {
        //A reflex vertex cant be an ear!
        if (v.isReflex)
        {
            return;
        }

        //This triangle to check point in triangle
        Vector2 a = v.prevVertex.GetPos2D_XZ();
        Vector2 b = v.GetPos2D_XZ();
        Vector2 c = v.nextVertex.GetPos2D_XZ();

        bool hasPointInside = false;

        for (int i = 0; i < vertices.Count; i++)
        {
            //We only need to check if a reflex vertex is inside of the triangle
            if (vertices[i].isReflex)
            {
                Vector2 p = vertices[i].GetPos2D_XZ();

                //This means inside and not on the hull
                if (IsPointInTriangle(a, b, c, p))
                {
                    hasPointInside = true;

                    break;
                }
            }
        }

        if (!hasPointInside)
        {
            earVertices.Add(v);
        }
    }

    public static int ClampListIndex(int index, int listSize)
    {
        index = ((index % listSize) + listSize) % listSize;

        return index;
    }

    public static bool IsPointInTriangle(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p)
    {
        bool isWithinTriangle = false;

        //Based on Barycentric coordinates
        float denominator = ((p2.y - p3.y) * (p1.x - p3.x) + (p3.x - p2.x) * (p1.y - p3.y));

        float a = ((p2.y - p3.y) * (p.x - p3.x) + (p3.x - p2.x) * (p.y - p3.y)) / denominator;
        float b = ((p3.y - p1.y) * (p.x - p3.x) + (p1.x - p3.x) * (p.y - p3.y)) / denominator;
        float c = 1 - a - b;

        //The point is within the triangle or on the border if 0 <= a <= 1 and 0 <= b <= 1 and 0 <= c <= 1
        //if (a >= 0f && a <= 1f && b >= 0f && b <= 1f && c >= 0f && c <= 1f)
        //{
        //    isWithinTriangle = true;
        //}

        //The point is within the triangle
        if (a > 0f && a < 1f && b > 0f && b < 1f && c > 0f && c < 1f)
        {
            isWithinTriangle = true;
        }

        return isWithinTriangle;
    }

    public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        bool isClockWise = true;

        float determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;

        if (determinant > 0f)
        {
            isClockWise = false;
        }

        return isClockWise;
    }
}

public class Triangle
{
    //Corners
    public Vertex v1;
    public Vertex v2;
    public Vertex v3;


    public Triangle(Vertex v1, Vertex v2, Vertex v3)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
    }

    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        this.v1 = new Vertex(v1);
        this.v2 = new Vertex(v2);
        this.v3 = new Vertex(v3);
    }



    //Change orientation of triangle from cw -> ccw or ccw -> cw
    public void ChangeOrientation()
    {
        Vertex temp = this.v1;

        this.v1 = this.v2;

        this.v2 = temp;
    }
}

public class Intersection
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

public class Vertex
{
    public Vector3 position;

    //The outgoing halfedge (a halfedge that starts at this vertex)
    //Doesnt matter which edge we connect to it

    //Which triangle is this vertex a part of?
    public Triangle triangle;

    //The previous and next vertex this vertex is attached to
    public Vertex prevVertex;
    public Vertex nextVertex;

    //Properties this vertex may have
    //Reflex is concave
    public bool isReflex;
    public bool isConvex;
    public bool isEar;

    public Vertex(Vector3 position)
    {
        this.position = position;
    }

    //Get 2d pos of this vertex
    public Vector2 GetPos2D_XZ()
    {
        Vector2 pos_2d_xz = new Vector2(position.x, position.z);

        return pos_2d_xz;
    }
}

