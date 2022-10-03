using System;
using System.Collections.Generic;
using DefaultNamespace.Economy;
using DefaultNamespace.Events;
using Pathfinding;
using Plugins.Ext;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
    public EconomyController EconomyController;
    public bool IsDrag;
    public Button BBuyBus;
    public Button BUseBus;
    public TMP_Text TBusCount;
    public int BusCount;
    public bool CanUseBus;
    public Location StartingLocation;
    public List<Location> WayPoints;
    public List<NodeView> WayPointNodes;
    public Location CurrentLocation;

    public override void OnEnter() {
      BusCount = 1;
      Canvas.enabled = true;
      EconomyController.Init();
      BBuyBus.onClick.AddListener(BuyBus);
      BUseBus.onClick.AddListener(UseBus);
    }

    void UseBus() {
      if (BusCount == 0) return;
      
      if (CanUseBus) {
        BusCount++;
        CanUseBus = false;
      }
      else {
        BusCount--;
        CanUseBus = true;
      }
    }

    void BuyBus() {
      BusCount++;
    }

    public override void OnExit() {
      BBuyBus.onClick.RemoveListener(BuyBus);
      BUseBus.onClick.RemoveListener(UseBus);
      ClearCars();
      ClearVisualization();
      wayPoints.Clear();
      WayPoints.Clear();
      WayPointNodes.Clear();
      path.Clear();
      Canvas.enabled = false;
      EPathBuilder = EPathBuilder.SelectLocationFrom;
      EconomyController.ResetState();
    }

    // public void GetClosestLocation(Location location) {
    //   float minDistance = float.MaxValue;
    //   var nodeModel = Grid.Nodes[0, 0].Model;
    //   Location closestLocation = new Location(nodeModel.X, nodeModel.Y);
    //   
    //   foreach (NodeView location in Grid.Nodes) {
    //     var distance = Vector3.Distance(new Vector3(location.x, 0, location.y), transform.position);
    //     if (distance < minDistance) {
    //       minDistance = distance;
    //       closestLocation = location;
    //     }
    //   }
    // }

    public override void OnUpdate() {
      TBusCount.text = $"Bus count: {BusCount}";
      
      // if (!Input.GetMouseButtonDown(0)) return;
      
      int mask = LayerMask.GetMask(NodeLayers);
      Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
      bool isHit = Physics.Raycast(ray, out RaycastHit hit, 100 /*, mask*/);

      if (!isHit || UIExt.IsPointerOverUIElement()) return;
      NodeView view = hit.transform.GetComponent<NodeView>();

      if (view == null) return;
      if (view.Model.ENode != ENode.Road) return;

      Location location = new Location(view.Model.X, view.Model.Y);
      
      if (Input.GetMouseButtonDown(0)) {
        if (CanUseBus && view.Model.ENode == ENode.Road) {
          Debug.Log("start");

          CanUseBus = false;
          IsDrag = true;
          StartingLocation = location;
          CurrentLocation = StartingLocation;
          WayPoints = new List<Location> {StartingLocation};
          WayPointNodes = new List<NodeView> {view};
          view.Highlight();
        }
      }

      int xDiff = Mathf.Abs(CurrentLocation.x - view.Model.X); 
      int yDiff = Mathf.Abs(CurrentLocation.y - view.Model.Y);
      Debug.Log(xDiff + " " + yDiff);
      if (xDiff > 1
          || yDiff > 1
          || xDiff > 0 && yDiff > 0) {
        //show message then selecting works only for closest tiles
      }
      else if (IsDrag && CurrentLocation != location && !WayPoints.Contains(location)) {
        CurrentLocation = location;
        WayPoints.Add(location);
        WayPointNodes.Add(view);
        view.Highlight();
      }
      
      if (Input.GetMouseButtonUp(0) && IsDrag) {
        Debug.Log($"finish: {WayPoints.Count}");
        if (WayPoints.Count <= 1) {
          UseBus();
        }
        else {
          path.AddRange(WayPoints);
          SpawnBus(view);
        }

        foreach (var node in WayPointNodes) {
          node.Unhighlight();
        }
        
        WayPoints.Clear();
        WayPointNodes.Clear();
        wayPoints.Clear();
        path.Clear();
        IsDrag = false;
      }
      // if (!IsDrag) return;
      return;
      switch (EPathBuilder) {
        case EPathBuilder.SelectLocationFrom:
          From = location;
          if (IsDebugEnabled) {
            GameObject startPoint = Instantiate(StartingPointPrefab, new Vector3(view.Model.X, 0, view.Model.Y),
              Quaternion.identity, transform);
            PathVisualization.Add(startPoint);
          }

          EPathBuilder = EPathBuilder.SelectLocationTo;
          break;
        case EPathBuilder.SelectLocationTo:
          To = location;
            
          if (From == To) {
            EPathBuilder = EPathBuilder.Reset;
            break;
          }

          aStarSearch = new AStarSearch(Grid, From, To);
          aStarSearch.Calculate();
          path = aStarSearch.GetCleanPath();
          path.Reverse();

          if (SpawnBus(view)) return;
          
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

    bool SpawnBus(NodeView view) {
      wayPoints = Grid.GetWayPoints(path);

      if (wayPoints.Count <= 0) {
        Debug.LogError("No waypoints");
        return true;
      }

      GameObject car = Instantiate(CarPrefab, wayPoints[wayPoints.Count - 1], Quaternion.identity, transform);
      CarView carView = car.AddComponent<CarView>();
      carView.SetGoal(wayPoints, path);
      carView.ColliderInFront = carView.transform.Find("ColliderInFront").GetComponent<BoxCollider>();
      carView.ColliderInRight = carView.transform.Find("ColliderInRight").GetComponent<BoxCollider>();
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

      EventController.BusBought();
      return false;
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