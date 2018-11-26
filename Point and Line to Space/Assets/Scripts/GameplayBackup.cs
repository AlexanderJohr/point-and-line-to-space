//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class GameplayBachup : MonoBehaviour
//{
//    public LineRenderer linePrefab;
//    public MeshFilter shapePrefab;

//    public String myName;

//    public float drawDistanceToCamera;

//    private Line currentLine;

//    private List<Line> drawnLines = new List<Line>();
//    private List<Shape> drawnShapes = new List<Shape>();

//    List<Vector2> intersectionPoints = new List<Vector2>();

//    public bool infiniteLength = true;
//    Camera screenCamera;

//    public AudioSource audioData;

//    public AudioSource exampleSound;




//    public int desiredIntersectionPointCount = 3;

//    void Start()
//    {
//        screenCamera = GetComponent<Camera>();
//    }

//    RaycastHit2D hit;
//    Vector3[] touches = new Vector3[5];






//    void Update()
//    {
//        foreach (Line line in drawnLines)
//        {
//            line.LineIntersections.Clear();
//        }
//        if (Input.touchCount >= 2)
//        {
//            foreach (Touch t in Input.touches)
//            {
//                Vector2 position = Input.GetTouch(t.fingerId).position;
//                touches[t.fingerId] = screenCamera.ScreenToWorldPoint(position) + transform.forward * drawDistanceToCamera;
//            }

//            Vector3 start = touches[0];
//            Vector3 end = touches[1];

//            if (currentLine == null)
//            {
//                currentLine = new Line(Instantiate<LineRenderer>(linePrefab));
//            }

//            if (infiniteLength)
//            {
//                Vector3 lineVector = start - end;
//                Vector3 infiniteEnd = end + (lineVector * 1000);
//                Vector3 infiniteStart = start - (lineVector * 1000);
//                start = infiniteStart;
//                end = infiniteEnd;
//            }

//            currentLine.Start = start;
//            currentLine.End = end;
//        }
//        else
//        {
//            if (currentLine != null)
//            {
//                drawnLines.Add(currentLine);
//                currentLine = null;
//            }
//        }

//        intersectionPoints.Clear();
//        List<Line> alreadyCheckedLines = new List<Line>();
//        int intersectingPointsCount = 0;
//        foreach (Line a in drawnLines)
//        {
//            foreach (Line b in drawnLines) 
//            {
//                bool areNotTheSame = drawnLines.IndexOf(a) != drawnLines.IndexOf(b);
//                if (areNotTheSame)
//                {
//                    a.Start2D = screenCamera.WorldToScreenPoint(a.Start);
//                    a.End2D = screenCamera.WorldToScreenPoint(a.End);
//                    b.Start2D = screenCamera.WorldToScreenPoint(b.Start);
//                    b.End2D = screenCamera.WorldToScreenPoint(b.End);

//                    bool found;
//                    // I get all the intersection points here:
//                    Vector2 intersection = GetIntersectionPointCoordinates(a.Start2D, a.End2D, b.Start2D, b.End2D, out found);
//                    // I get all intersection points that are visible on screen
//                    if (found)
//                    {
//                        float dotProd = Vector2.Angle(a.Direction2D, b.Direction2D);
//                        print(dotProd);

//                        float dotProd2 = 180 -Vector2.Angle(a.Direction2D, -b.Direction2D);
//                        print(dotProd2);
//                        bool isInsideScreen = screenCamera.pixelRect.Contains(intersection);
//                        LineIntersection lineIntersection = new LineIntersection(b, dotProd, isInsideScreen);
//                        //    a.LineIntersection = lineIntersection;

//                        intersectingPointsCount++;

//                        intersectionPoints.Add(intersection);
//                    }
//                }
//            }
//            alreadyCheckedLines.Add(a);
//        }

//        List<Vector2> shapePointsWithMaxPoints = new List<Vector2>();
//        foreach (Line line in drawnLines)
//        {
//            List<Vector2> shapePoints = new List<Vector2> { line.Start2D };

//            bool foundCircle = false;
//            LineIntersection nextLineIntersection = line.LineIntersections.First();
//            while (nextLineIntersection != null && nextLineIntersection.IsInsideScreen)
//            {
//                LineIntersection lineInTwoSteps = lineInTwoSteps = nextLineIntersection.IntersectingLine.LineIntersections.First().IntersectingLine.LineIntersections.First();
//                bool lineComesBack = drawnLines.IndexOf(lineInTwoSteps.IntersectingLine) == drawnLines.IndexOf(nextLineIntersection.IntersectingLine);
//                if (lineComesBack)
//                {
//                    print("lineComesBack!");
//                    break;
//                }

//                shapePoints.Add(nextLineIntersection.IntersectingLine.Start2D);
//                bool areTheSame = drawnLines.IndexOf(line) == drawnLines.IndexOf(nextLineIntersection.IntersectingLine);
//                print(String.Format("a {0} and b {1} are {2}", drawnLines.IndexOf(line), drawnLines.IndexOf(nextLineIntersection.IntersectingLine), areTheSame ? "the same" : "not the same"));

//                if (areTheSame)
//                {
//                    print("foundCircle!");
//                    foundCircle = true;
//                    break;
//                }


//                nextLineIntersection = nextLineIntersection.IntersectingLine.LineIntersections.First();
//            }
//            if (foundCircle && shapePoints.Count > shapePointsWithMaxPoints.Count)
//            {
//                shapePointsWithMaxPoints = shapePoints;
//            }

//        }

//        if(shapePointsWithMaxPoints.Count > 2){
//            List<Vector3> convexHull3D = shapePointsWithMaxPoints.Select(v => screenCamera.ScreenToWorldPoint(v) + transform.forward * drawDistanceToCamera).ToList();
//            Shape shape = new Shape(Instantiate<MeshFilter>(shapePrefab), convexHull3D);

//            drawnShapes.Add(shape);

//                    audioData.Play(0);
//        }

//        //List<Vector2> convexHull = JarvisMarchAlgorithm.GetConvexHull(intersectionPoints);


//        //    if (convexHull != null && convexHull.Count > desiredIntersectionPointCount)
//        //    {
//        //        List<Vector3> convexHull3D = convexHull.Select(v => screenCamera.ScreenToWorldPoint(v) + transform.forward * drawDistanceToCamera).ToList();

//        //Shape shape = new Shape(Instantiate<MeshFilter>(shapePrefab), convexHull3D);

//        //drawnShapes.Add(shape);

//        //        audioData.Play(0);

//        // drawnLines.ForEach((line) => Destroy(line.Renderer));
//        //        drawnLines.Clear();
//        //    }
//    }



//    /// <summary>
//    /// Gets the coordinates of the intersection point of two lines.
//    /// </summary>
//    /// <param name="A1">A point on the first line.</param>
//    /// <param name="A2">Another point on the first line.</param>
//    /// <param name="B1">A point on the second line.</param>
//    /// <param name="B2">Another point on the second line.</param>
//    /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
//    /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
//    private Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, out bool found)
//    {
//        float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

//        if (tmp == 0)
//        {
//            // No solution!
//            found = false;
//            return Vector2.zero;
//        }

//        float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

//        found = true;

//        return new Vector2(
//            B1.x + (B2.x - B1.x) * mu,
//            B1.y + (B2.y - B1.y) * mu
//        );
//    }

//    class Shape
//    {

//        public MeshFilter MeshFilter { get; private set; }

//        public Shape(MeshFilter meshFilter, List<Vector3> convexHull)
//        {
//            Mesh mesh = new Mesh();
//            meshFilter.mesh = mesh;

//            meshFilter.mesh.vertices = convexHull.ToArray();

//            List<Triangle> triangles = JarvisMarchAlgorithm.TriangulateConvexPolygon(convexHull);
//            int[] tri = new int[triangles.Count * 3];
//            for (int i = 0; i < triangles.Count; i++)
//            {

//                tri[i * 3] = 0;
//                tri[(i * 3) + 1] = i + 1;
//                tri[(i * 3) + 2] = i + 2;



//            }


//            Vector3[] normals = new Vector3[convexHull.Count];
//            for (int i = 0; i < normals.Count(); i++)
//            {
//                normals[i] = Vector3.forward;
//            }


//            meshFilter.mesh.normals = normals;



//            meshFilter.mesh.triangles = tri;


//            this.MeshFilter = meshFilter;

//        }

//    }
//}

//class LineIntersection
//{
//    public Line IntersectingLine { get; private set; }
//    public float DotProd { get; private set; }
//    public bool IsInsideScreen { get; private set; }


//    public LineIntersection(Line intersectingLine, float dotProd, bool isInsideScreen)
//    {
//        this.IntersectingLine = intersectingLine;
//        this.DotProd = dotProd;
//        this.IsInsideScreen = isInsideScreen;
//    }
//}

//class Line
//{
//    public Vector2 Start2D
//    {
//        get;
//        set;
//    }
//    public Vector2 Direction2D
//    {
//        get { return End2D - Start2D; }
//    }

//    private Vector3 _start;
//    public Vector3 Start
//    {
//        get
//        {
//            return _start;
//        }
//        set
//        {
//            _start = value;
//            Renderer.SetPosition(0, _start);
//        }
//    }

//    public Vector2 End2D
//    {
//        get;
//        set;
//    }

//    private Vector3 _end;

//    public Vector3 End
//    {
//        get
//        {
//            return _end;
//        }
//        set
//        {
//            _end = value;
//            Renderer.SetPosition(1, _end);
//        }
//    }

//    public Vector3 Direction
//    {
//        get
//        {
//            return _end - _start;
//        }
//    }

//    private LineIntersection lineIntersection = null;

//    public List<LineIntersection> LineIntersections
//    {
//        get;
//        set;
//    }
//    public LineIntersection FirstLineIntersection
//    {
//        get
//        {
//            return lineIntersection;
//        }
//        set
//        {
//            if (lineIntersection == null || value == null || value.DotProd < lineIntersection.DotProd)
//            {
//                lineIntersection = value;
//            }
//        }
//    }

//    public LineRenderer Renderer { get; private set; }

//    public Line(LineRenderer renderer)
//    {
//        this.Renderer = renderer;
//        LineIntersections = new List<LineIntersection>();
//    }
//}



//public static class JarvisMarchAlgorithm
//{
//    public static List<Vector2> GetConvexHull(List<Vector2> points)
//    {
//        //If we have just 3 points, then they are the convex hull, so return those
//        if (points.Count == 3)
//        {
//            //These might not be ccw, and they may also be colinear
//            return points;
//        }

//        //If fewer points, then we cant create a convex hull
//        if (points.Count < 3)
//        {
//            return null;
//        }



//        //The list with points on the convex hull
//        List<Vector2> convexHull = new List<Vector2>();

//        //Step 1. Find the vertex with the smallest x coordinate
//        //If several have the same x coordinate, find the one with the smallest z
//        Vector2 startVertex = points[0];


//        for (int i = 1; i < points.Count; i++)
//        {
//            Vector2 testPos = points[i];

//            if (testPos.x < startVertex.x)
//            {
//                startVertex = points[i];

//            }
//        }

//        //This vertex is always on the convex hull
//        convexHull.Add(startVertex);

//        points.Remove(startVertex);



//        //Step 2. Loop to generate the convex hull
//        Vector2 currentPoint = convexHull[0];

//        //Store colinear points here - better to create this list once than each loop
//        List<Vector2> colinearPoints = new List<Vector2>();

//        int counter = 0;

//        while (true)
//        {
//            //After 2 iterations we have to add the start position again so we can terminate the algorithm
//            //Cant use convexhull.count because of colinear points, so we need a counter
//            if (counter == 2)
//            {
//                points.Add(convexHull[0]);
//            }

//            //Pick next point randomly
//            Vector2 nextPoint = points[UnityEngine.Random.Range(0, points.Count)];

//            //To 2d space so we can see if a point is to the left is the vector ab
//            Vector2 a = currentPoint;

//            Vector2 b = nextPoint;

//            //Test if there's a point to the right of ab, if so then it's the new b
//            for (int i = 0; i < points.Count; i++)
//            {
//                //Dont test the point we picked randomly
//                if (points[i].Equals(nextPoint))
//                {
//                    continue;
//                }

//                Vector2 c = points[i];

//                //Where is c in relation to a-b
//                // < 0 -> to the right
//                // = 0 -> on the line
//                // > 0 -> to the left
//                float relation = IsAPointLeftOfVectorOrOnTheLine(a, b, c);

//                //Colinear points
//                //Cant use exactly 0 because of floating point precision issues
//                //This accuracy is smallest possible, if smaller points will be missed if we are testing with a plane
//                float accuracy = 0.00001f;

//                if (relation < accuracy && relation > -accuracy)
//                {
//                    colinearPoints.Add(points[i]);
//                }
//                //To the right = better point, so pick it as next point on the convex hull
//                else if (relation < 0f)
//                {
//                    nextPoint = points[i];

//                    b = nextPoint;

//                    //Clear colinear points
//                    colinearPoints.Clear();
//                }
//                //To the left = worse point so do nothing
//            }



//            //If we have colinear points
//            if (colinearPoints.Count > 0)
//            {
//                colinearPoints.Add(nextPoint);

//                //Sort this list, so we can add the colinear points in correct order
//                colinearPoints = colinearPoints.OrderBy(n => Vector3.SqrMagnitude(n - currentPoint)).ToList();

//                convexHull.AddRange(colinearPoints);

//                currentPoint = colinearPoints[colinearPoints.Count - 1];

//                //Remove the points that are now on the convex hull
//                for (int i = 0; i < colinearPoints.Count; i++)
//                {
//                    points.Remove(colinearPoints[i]);
//                }

//                colinearPoints.Clear();
//            }
//            else
//            {
//                convexHull.Add(nextPoint);

//                points.Remove(nextPoint);

//                currentPoint = nextPoint;
//            }

//            //Have we found the first point on the hull? If so we have completed the hull
//            if (currentPoint.Equals(convexHull[0]))
//            {
//                //Then remove it because it is the same as the first point, and we want a convex hull with no duplicates
//                convexHull.RemoveAt(convexHull.Count - 1);

//                break;
//            }

//            counter += 1;
//        }

//        return convexHull;
//    }

//    public static float IsAPointLeftOfVectorOrOnTheLine(Vector2 a, Vector2 b, Vector2 p)
//    {
//        float determinant = (a.x - p.x) * (b.y - p.y) - (a.y - p.y) * (b.x - p.x);

//        return determinant;
//    }

//    public static List<Triangle> TriangulateConvexPolygon(List<Vector3> convexHullpoints)
//    {
//        List<Triangle> triangles = new List<Triangle>();

//        for (int i = 2; i < convexHullpoints.Count; i++)
//        {
//            Vector3 a = convexHullpoints[0];
//            Vector3 b = convexHullpoints[i - 1];
//            Vector3 c = convexHullpoints[i];

//            triangles.Add(new Triangle(a, b, c));
//        }

//        return triangles;
//    }


//}

//public class Triangle
//{
//    //Corners
//    public Vector3 v1;
//    public Vector3 v2;
//    public Vector3 v3;

//    public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
//    {
//        this.v1 = v1;
//        this.v2 = v2;
//        this.v3 = v3;
//    }
//}





//// same / equal to: 

////public class Student
////{
////    private string _name;
////    public string Name
////    {
////        get
////        {
////            return _name;
////        }
////        private set
////        {
////            _name = value;
////        }
////    }
////}
