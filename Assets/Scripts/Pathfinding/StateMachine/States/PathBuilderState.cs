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
    public GameObject CarPrefab;
    public List<GameObject> PathVisualization = new List<GameObject>();
    public List<GameObject> Cars = new List<GameObject>();
    public Canvas Canvas;
    public Camera Camera;
    public Location From;
    public Location To;
    public EPathBuilder EPathBuilder;
    public string[] NodeLayers;
    AStarSearch aStarSearch;
    List<Location> path = new List<Location>();
    List<Vector3> wayPoints = new List<Vector3>();
    public bool IsDebugEnabled;

    public override void OnEnter() {
      Canvas.enabled = true;
    }

    public override void OnExit() {
      ClearCars();
      ClearVisualization();
      Canvas.enabled = false;
      EPathBuilder = EPathBuilder.SelectLocationFrom;
    }

    public override void OnUpdate() {
      if (!Input.GetMouseButtonDown(0)) return;
      
      int mask = LayerMask.GetMask(NodeLayers);
      Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
      bool isHit = Physics.Raycast(ray, out RaycastHit hit, 100 /*, mask*/);

      if (!isHit || UIExt.IsPointerOverUIElement()) return;
      NodeView view = hit.transform.GetComponent<NodeView>();

      if (view == null) return;
        
      switch (EPathBuilder) {
        case EPathBuilder.SelectLocationFrom:
          From = new Location(view.Model.X, view.Model.Y);
          if (IsDebugEnabled) {
            GameObject startPoint = Instantiate(StartingPointPrefab, new Vector3(view.Model.X, 0, view.Model.Y),
              Quaternion.identity, transform);
            PathVisualization.Add(startPoint);
          }

          EPathBuilder = EPathBuilder.SelectLocationTo;
          break;
        case EPathBuilder.SelectLocationTo:
          To = new Location(view.Model.X, view.Model.Y);
            
          if (From == To) {
            EPathBuilder = EPathBuilder.Reset;
            break;
          }

          aStarSearch = new AStarSearch(Grid, From, To);
          aStarSearch.Calculate();
          path = aStarSearch.GetCleanPath();
          path.Reverse();
          wayPoints = Grid.GetWayPoints(path);
            
          if (wayPoints.Count <= 0) {
            Debug.LogError("No waypoints");
            return;
          }
            
          GameObject car = Instantiate(CarPrefab, wayPoints[wayPoints.Count - 1], Quaternion.identity, transform);
          CarView carView = car.AddComponent<CarView>();
          carView.SetGoal(wayPoints);
          carView.ColliderInFront = carView.transform.Find("ColliderInFront").GetComponent<BoxCollider>();
          Cars.Add(car);
            
          if (IsDebugEnabled) {
            GameObject endPoint = Instantiate(EndPointPrefab, new Vector3(view.Model.X, 0, view.Model.Y),
              Quaternion.identity, transform);
            PathVisualization.Add(endPoint);
              
            for (int i = 0; i < wayPoints.Count; i++) {
              int previousIndex = i == 0 ? wayPoints.Count - 1 : i - 1;
              Vector3 from = wayPoints[previousIndex];
              Vector3 to = wayPoints[i];
              GameObject line = DrawLine(@from, to, Color.green);

              line.transform.SetParent(transform);
              PathVisualization.Add(line);
            }
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
    
    void ClearCars() {
      foreach (GameObject obj in Cars) {
        Destroy(obj);
      }

      Cars.Clear();
    }
    
    void OnDrawGizmos() {
      if (aStarSearch == null) return;
      if (!IsDebugEnabled) return;
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