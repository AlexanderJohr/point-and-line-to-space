using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateLocalPlayer : NetworkBehaviour
{

    Vector3?[] touches = new Vector3?[2];
    Camera camera;

    public Line CurrentLine { get; set; }
    public Line CurrentNetworkLine { get; set; }

    public Line linePrefab;
    Vector3? StartPoint;
    Vector3? EndPoint;

    void Start()
    {

        if (!isLocalPlayer) return;
        camera = GetComponentsInChildren<Camera>()[0];
        AudioListener audioListener = GetComponentsInChildren<AudioListener>()[0];

        camera.enabled = true;
        audioListener.enabled = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.touchCount >= 2)
        {
            foreach (Touch t in Input.touches)
            {
                Vector2 position = Input.GetTouch(t.fingerId).position;
                touches[t.fingerId] = camera.ScreenToWorldPoint(position) + transform.forward * 100;
            }

            StartPoint = touches[0];
            EndPoint = touches[1];
        }
        else
        {
            StartPoint = null;
            EndPoint = null;
        }

        if (Input.GetMouseButton(0))
        {
            if (touches[0] == null)
            {
                touches[0] = camera.ScreenToWorldPoint(Input.mousePosition) + transform.forward * 100;
            }
            else
            {
                touches[1] = camera.ScreenToWorldPoint(Input.mousePosition) + transform.forward * 100;
            }

            if (touches[0] != null && touches[1] != null) {
                StartPoint = touches[0];
                EndPoint = touches[1];
            }
        }
        else
        {
            touches[0] = null;
            touches[1] = null;
        }





        if (CurrentLine == null && (StartPoint != null || EndPoint != null))
        {
            CurrentLine = Instantiate(linePrefab);
            
            currentNetworkLinesCount = currentNetworkLines.Count;

            currentNetworkLines.Add(currentNetworkLinesCount, CurrentLine);

            CmdSpawnLine(currentNetworkLinesCount, StartPoint.Value, EndPoint.Value);
        }

        if (CurrentLine != null)
        {
            if (StartPoint != null)
            {
                CurrentLine.StartPoint = StartPoint.Value;
                CmdUpdateLine(currentNetworkLinesCount, EndPoint.Value, StartPoint.Value);
            }
            if (EndPoint != null)
            {
                CurrentLine.EndPoint = EndPoint.Value;
                CmdUpdateLine(currentNetworkLinesCount, EndPoint.Value, StartPoint.Value);

            }
            if (StartPoint == null && EndPoint == null)
            {
                CurrentLine = null;
            }
            

        }
    }

    int currentNetworkLinesCount = 0;
    Dictionary<int, Line> currentNetworkLines = new Dictionary<int, Line>();

    [Command]
    void CmdSpawnLine(int id,  Vector3 startPoint, Vector3 endPoint)
    {

        //CurrentNetworkLine = Instantiate(linePrefab);

        //currentNetworkLines.Add(id, CurrentNetworkLine);

        //NetworkServer.SpawnWithClientAuthority(CurrentNetworkLine.gameObject, base.connectionToClient);
        RpcSpawnLine(id, startPoint, endPoint);
        //RpcSendIdToClient(id);
    }

    [ClientRpc]
    void RpcSpawnLine(int id, Vector3 startPoint, Vector3 endPoint)
    {
        bool lineIsNotYetCreatedOnThisCLient = !currentNetworkLines.ContainsKey(id);
        if (lineIsNotYetCreatedOnThisCLient) {
            CurrentLine = Instantiate(linePrefab);
            CurrentLine.StartPoint = startPoint;
            CurrentLine.EndPoint = endPoint;

            currentNetworkLines.Add(id, CurrentLine);
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
        if (currentNetworkLines.ContainsKey(id)) {
            Line line = currentNetworkLines[id];
            line.StartPoint = startPoint;
            line.EndPoint = endPoint;
        }
       
    }



}
