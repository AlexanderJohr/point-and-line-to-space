using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateLocalPlayer : NetworkBehaviour
{

    Vector3?[] touches = new Vector3?[2];
    Camera screenCamera;
    List<Intersection> intersections = new List<Intersection>();

    public PlayerData data;

    public Shape shapePrefab;

    public Line linePrefab;
    public BackgroundAudioMixerControl backgroundAudioMixerControl;

    void Start()
    {

        if (isLocalPlayer)
        {            data.GameOver = false;

            screenCamera = GetComponentsInChildren<Camera>()[0];
            AudioListener audioListener = GetComponentsInChildren<AudioListener>()[0];

            screenCamera.enabled = true;
            audioListener.enabled = true;

            data.Ink = 100;
            data.Score = 0;
            data.RemainingSeconds = 60;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || data.GameOver) return;

        data.RemainingSeconds -= Time.deltaTime;

        if (data.RemainingSeconds < 20)
        {
            backgroundAudioMixerControl.TransitionTo(4);
        }

        if (data.RemainingSeconds < 0) {
            data.GameOver = true;
            CmdSubmitScore((int)data.Score);
        }


        float maxInk = 100f;
        float inkReplenishMultiplier = 0.005f;
        if (data.Ink < 100)
        {
            data.Ink += (float)Math.Pow(maxInk - data.Ink, 2) * inkReplenishMultiplier * Time.deltaTime + 20 * Time.deltaTime;
            if (data.Ink > 100)
            {
                data.Ink = 100;
            }
        }



        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            if (Input.touchCount >= 2)
            {
                foreach (Touch t in Input.touches)
                {
                    Vector2 position = Input.GetTouch(t.fingerId).position;
                    touches[t.fingerId] = GetWorldPositionOnPlane(screenCamera, position, 100);
                }

                data.StartPoint = touches[0];
                data.EndPoint = touches[1];
            }
            else
            {
                data.StartPoint = null;
                data.EndPoint = null;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                if (data.StartPoint == null)
                {
                    data.StartPoint2D = Input.mousePosition;
                }
                else
                {
                    data.EndPoint2D = Input.mousePosition;
                }

                data.StartPoint = GetWorldPositionOnPlane(screenCamera, data.StartPoint2D, 100);
                data.EndPoint = GetWorldPositionOnPlane(screenCamera, data.EndPoint2D, 100);

            }
            else
            {
                data.StartPoint = null;
                data.EndPoint = null;
            }
        }

        if (data.CurrentLine == null && (data.StartPoint != null && data.EndPoint != null))
        {
            data.CurrentLine = Instantiate(linePrefab);
            data.CurrentLine.PlayerId = 1;
            data.CurrentLine.IsLocalLine = true;
            data.CurrentLine.IsReleased = false;
            data.CurrentLine.Id = data.localPlayerLineIndex++;

            CmdSpawnLine(data.CurrentLine.Id, data.StartPoint.Value, data.EndPoint.Value);
        }

        if (data.CurrentLine != null)
        {
            if (data.StartPoint != null && data.EndPoint != null)
            {
                data.CurrentLine.StartPoint = data.StartPoint.Value;
                data.CurrentLine.EndPoint = data.EndPoint.Value;

                data.InkNeededForCurrentLine = data.CurrentLine.Magnitude;
                if (data.CurrentLine.Magnitude > data.Ink)
                {
                    data.CurrentLine.InsufficientInk = true;
                }
                else
                {
                    data.CurrentLine.InsufficientInk = false;
                }



                CmdUpdateLine(data.CurrentLine.Id, data.EndPoint.Value, data.StartPoint.Value);
            }

            if (data.StartPoint == null && data.EndPoint == null)
            {
                data.CurrentLine.IsReleased = true;

                if (data.CurrentLine.InsufficientInk)
                {
                    data.CurrentLine.Die();
                }
                else if (data.CurrentLine.IntersectsShape)
                {
                    data.CurrentLine.Die();
                    data.Ink -= (int)data.CurrentLine.Magnitude;
                }
                else
                {
                    data.localPlayerLines.Add(data.CurrentLine.Id, data.CurrentLine);
                    data.Ink -= (int)data.CurrentLine.Magnitude;
                }
                data.CurrentLine = null;
                data.InkNeededForCurrentLine = 0;
            }
        }

        intersections.Clear();
        List<Line> alreadyCheckedLines = new List<Line>();
        List<Line> drawnLines = data.localPlayerLines.Values.ToList();
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

            Vector3[] convexHull3D = maxVertexList.Select(v => GetWorldPositionOnPlane(screenCamera, v, 100)).ToArray();

            List<Triangle> triangles = TriangulateConvexPolygon(convexHull3D);
            var score = triangles.Sum(i => i.SurfaceArea);

            if (score > 2) {
                data.Score += score;

                Shape shape = Instantiate(shapePrefab);
                shape.PlayerId = 1;
                shape.ConvexHull = convexHull3D;


                shape.Id = data.localPlayerShapeIndex++;
                data.localPlayerShapes.Add(shape.Id, shape);

                CmdSpawnShape(shape.Id, convexHull3D.ToArray());

                // audioData.Play(0);

                data.localPlayerLines.Values.ToList().ForEach((line) => Destroy(line));
                data.localPlayerLines.Clear();
                data.localPlayerLines.Clear();

                CmdDeleteOtherUserLines();
            }

        }

        List<KeyValuePair<int, Line>> linesToDestroy = new List<KeyValuePair<int, Line>>(); ;
        foreach (KeyValuePair<int, Line> lineKeyValuePair in data.localPlayerLines)
        {
            bool lineIntersectsShape = checkIfLineIntersectsAnyShape(lineKeyValuePair.Value);

            if (lineIntersectsShape)
            {
                linesToDestroy.Add(lineKeyValuePair);
            }

        }





        linesToDestroy.ForEach((lineKeyValuePair) =>
        {
            lineKeyValuePair.Value.Die();
            data.localPlayerLines.Remove(lineKeyValuePair.Key);
            CmdDeleteOtherUserLine(lineKeyValuePair.Value.Id);
        });

        if (data.CurrentLine != null)
        {
            bool currentLineIntersectsShape = checkIfLineIntersectsAnyShape(data.CurrentLine);
            if (currentLineIntersectsShape)
            {
                data.CurrentLine.IntersectsShape = true;
            }
            else
            {
                data.CurrentLine.IntersectsShape = false;
            }
        }


    }


    private bool checkIfLineIntersectsAnyShape(Line line)
    {
        var allShapes = data.otherPlayerShapes.Values.Concat(data.localPlayerShapes.Values);

        foreach (Shape shape in allShapes)
        {
            Vector3[] convexHull = shape.ConvexHull;

            for (int i = 0; i < convexHull.Length - 1; i++)
            {
                Vector2 a1 = screenCamera.WorldToScreenPoint(convexHull[i]);
                Vector2 a2 = screenCamera.WorldToScreenPoint(convexHull[i + 1]);
                Vector2 b1 = screenCamera.WorldToScreenPoint(line.StartPoint);
                Vector2 b2 = screenCamera.WorldToScreenPoint(line.EndPoint);

                bool found;
                Vector2 intersectionPoint = GetIntersectionPointCoordinates(a1, a2, b1, b2, out found);

                if (found)
                {
                    bool isInLineSegmentA = CheckPointIsInLineSegment(a1, a2, intersectionPoint);
                    bool isInLineSegmentB = CheckPointIsInLineSegment(b1, b2, intersectionPoint);

                    if (isInLineSegmentA && isInLineSegmentB)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    [Command]
    private void CmdSpawnShape(int id, Vector3[] convexHull3D)
    {
        RpcSpawnShape(id, convexHull3D);
    }
    [ClientRpc]
    private void RpcSpawnShape(int id, Vector3[] convexHull3D)
    {
        if (!isLocalPlayer)
        {
            Shape shape = Instantiate(shapePrefab);

            shape.PlayerId = 2;
            shape.ConvexHull = convexHull3D;
            data.otherPlayerShapes.Add(id, shape);
        }
    }

    [Command]
    void CmdSubmitScore(int  score)
    {
        RpcReceiveScore(score);
    }
    [ClientRpc]
    void RpcReceiveScore(int score)
    {
        if (!isLocalPlayer)
        {
            data.OtherPlayerScore = score;

        }
    }

    [Command]
    void CmdSpawnLine(int id, Vector3 startPoint, Vector3 endPoint)
    {
        RpcSpawnLine(id, startPoint, endPoint);
    }

    [ClientRpc]
    void RpcSpawnLine(int id, Vector3 startPoint, Vector3 endPoint)
    {
        if (!isLocalPlayer)
        {
            Line line = Instantiate(linePrefab);
            line.PlayerId = 2;
            line.StartPoint = startPoint;
            line.EndPoint = endPoint;

            data.otherPlayerLines.Add(id, line);
        }
    }

    [Command]
    void CmdUpdateLine(int id, Vector3 startPoint, Vector3 endPoint)
    {
        RpcUpdateLine(id, startPoint, endPoint);
    }

    [ClientRpc]
    void RpcUpdateLine(int id, Vector3 startPoint, Vector3 endPoint)
    {
        if (!isLocalPlayer)
        {
            Line line = data.otherPlayerLines[id];

            line.StartPoint = startPoint;
            line.EndPoint = endPoint;
        }
    }

    [Command]
    void CmdDeleteOtherUserLines()
    {
        RpcDeleteOtherUserLines();
    }

    [ClientRpc]
    void RpcDeleteOtherUserLines()
    {
        foreach (var l in data.otherPlayerLines.Values)
        {
            GameObject.Destroy(l);
        }
        data.otherPlayerLines.Clear();
    }

    [Command]
    void CmdDeleteOtherUserLine(int id)
    {
        RpcDeleteOtherUserLine(id);
    }

    [ClientRpc]
    void RpcDeleteOtherUserLine(int id)
    {
        if (!isLocalPlayer)
        {
            GameObject.Destroy(data.otherPlayerLines[id]);
            data.otherPlayerLines.Remove(id);
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
        public float SurfaceArea
        {
            get
            {
                Vector3 a = v2 - v1;
                Vector3 b = v3 - v1;
                return Vector3.Cross(a, b).magnitude;
            }
        }
        public void printAngles()
        {
            Vector3 a = v2 - v1;
            Vector3 b = v3 - v1;
            Vector3 c = v3 - v2;

            print(Vector3.Angle(a, b));
            Vector3.Angle(a, c);
            Vector3.Angle(b, c);

        }

    }

    public static List<Triangle> TriangulateConvexPolygon(Vector3[] convexHullpoints)
    {
        List<Triangle> triangles = new List<Triangle>();

        for (int i = 2; i < convexHullpoints.Length; i++)
        {
            Vector3 a = convexHullpoints[0];
            Vector3 b = convexHullpoints[i - 1];
            Vector3 c = convexHullpoints[i];

            triangles.Add(new Triangle(a, b, c));
        }

        return triangles;
    }










    public static Vector3 GetWorldPositionOnPlane(Camera camera, Vector3 screenPosition, float z)
    {
        Ray ray = camera.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(camera.transform.forward, camera.transform.position + (camera.transform.forward * z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

    public static Vector3 GetPlanePositionOfWorld(Camera camera, Vector3 worldPosition, float z)
    {
        Ray ray = camera.ViewportPointToRay(worldPosition);
        Plane xy = new Plane(camera.transform.forward, camera.transform.position + (camera.transform.forward * z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

}



public static class JarvisMarchAlgorithm
{

    


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
