using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Point and Line to Space/Player Data")]
public class PlayerData : ScriptableObject {

    public int localPlayerLineIndex = 0;
    public int localPlayerShapeIndex = 0;
    public float Score = 0;
    public float Ink = 100f;
    public float InkNeededForCurrentLine = 0f;
    public bool GameOver { get; set; }

    public float RemainingSeconds { get; set; }


    public Line CurrentLine { get; set; }
    public Line CurrentNetworkLine { get; set; }
    public int OtherPlayerScore { get; set; }


    public Dictionary<int, Line> otherPlayerLines = new Dictionary<int, Line>();
    public Dictionary<int, Line> localPlayerLines = new Dictionary<int, Line>();

    public Dictionary<int, Shape> otherPlayerShapes = new Dictionary<int, Shape>();
    public Dictionary<int, Shape> localPlayerShapes = new Dictionary<int, Shape>();


    
    public Vector3? StartPoint;
    public Vector3? EndPoint;

    public Vector3 StartPoint2D;
    public Vector3 EndPoint2D;
}
