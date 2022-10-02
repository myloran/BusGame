using System;
using System.Collections.Generic;
using Pathfinding;
using Plugins.Ext;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public class PathBuilderState : BaseState {
    public NGrid Grid;
    public GameObject StartingPointPrefab;
    public GameObject EndPointPrefab;
    public List<GameObject> PathVisualization = new List<GameObject>();
    public Canvas Canvas;
    public Camera Camera;
    public Location From;
    public Location To;
    public EPathBuilder EPathBuilder;
    public string[] NodeLayers;
    AStarSearch aStarSearch;
    List<Location> path = new List<Location>();

    public override void OnEnter() {
      Canvas.enabled = true;
    }

    public override void OnExit() {
      ClearVisualization();
      Canvas.enabled = false;
      EPathBuilder = EPathBuilder.SelectLocationFrom;
    }

    public override void OnUpdate() {
      if (Input.GetMouseButtonDown(0)) {
        int mask = LayerMask.GetMask(NodeLayers);
        Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, 100 /*, mask*/);

        if (!isHit || UIExt.IsPointerOverUIElement()) return;
        NodeView view = hit.transform.GetComponent<NodeView>();

        if (view == null) return;
        
        switch (EPathBuilder) {
          case EPathBuilder.SelectLocationFrom:
            From = new Location(view.Model.X, view.Model.Y);
            GameObject startPoint = Instantiate(StartingPointPrefab, new Vector3(view.Model.X, 0, view.Model.Y),
              Quaternion.identity, transform);
            PathVisualization.Add(startPoint);
            EPathBuilder = EPathBuilder.SelectLocationTo;
            break;
          case EPathBuilder.SelectLocationTo:
            To = new Location(view.Model.X, view.Model.Y);
            GameObject endPoint = Instantiate(EndPointPrefab, new Vector3(view.Model.X, 0, view.Model.Y),
              Quaternion.identity, transform);
            PathVisualization.Add(endPoint);
                
            aStarSearch = new AStarSearch(Grid, From, To);
            aStarSearch.Calculate();
                
            // foreach (KeyValuePair<Location, Location> location in aStarSearch.cameFrom) {
            //   Vector3 from = new Vector3(location.Key.x, 0, location.Key.y);
            //   Vector3 to = new Vector3(location.Value.x, 0, location.Value.y);
            //   GameObject line = DrawLine(from, to, Color.green);
            //   
            //   line.transform.SetParent(transform);
            //   PathVisualization.Add(line);
            // }

            path = aStarSearch.GetCleanPath();
                
            for (int i = 1; i < path.Count; i++) {
              Vector3 from = new Vector3(path[i-1].x, 0, path[i-1].y);
              Vector3 to = new Vector3(path[i].x, 0, path[i].y);
              GameObject line = DrawLine(@from, to, Color.green);
                  
              line.transform.SetParent(transform);
              PathVisualization.Add(line);
            }
                
            EPathBuilder = EPathBuilder.Reset;
            break;
          case EPathBuilder.Reset:
            ClearVisualization();
            EPathBuilder = EPathBuilder.SelectLocationFrom;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
    
    GameObject DrawLine(Vector3 start, Vector3 end, Color color/*, float duration = 0.2f*/)
    {
      GameObject myLine = new GameObject();
      myLine.transform.position = start;
      myLine.AddComponent<LineRenderer>();
      LineRenderer lr = myLine.GetComponent<LineRenderer>();
      // lr.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Lit"));//Alpha Blended Premultiply
      lr.SetColors(color, color);
      lr.SetWidth(0.1f, 0.1f);
      lr.SetPosition(0, start);
      lr.SetPosition(1, end);
      return myLine;
      // GameObject.Destroy(myLine, duration);
    }
    
    void ClearVisualization() {
      foreach (GameObject obj in PathVisualization) {
        Destroy(obj);
      }

      PathVisualization.Clear();
    }
    
    void OnDrawGizmos() {
      if (aStarSearch == null) return;
      if (EPathBuilder != EPathBuilder.Reset) return;

      foreach (var d in aStarSearch.costSoFar) {
        if (!path.Contains(d.Key)) continue;
        
        Handles.Label(new Vector3(d.Key.x, 0, d.Key.y), d.Value.ToString());
      }
    }
  }

  public enum EPathBuilder {
    SelectLocationFrom,
    SelectLocationTo,
    Reset,
  }
}