using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Pathfinding {
  public class NGrid : MonoBehaviour, WeightedGraph<Location> {
    public NodeView[,] Nodes;
    public NodeBuildingService NodeBuildingService;
    public GridState GridState;
    public Transform StartingPoint;
    public GameObject EmptyPrefab;
    public int SizeX;
    public int SizeY;
    public bool NeedLoad;
    public List<GridState> Levels = new List<GridState>();
    
    public readonly Location[] DIRS = new []
    {
      new Location(1, 0),
      new Location(0, -1),
      new Location(-1, 0),
      new Location(0, 1)
    };

    public void Init() {
      if (NeedLoad)
        Load();
      else
        Generate();
    }

    void Generate() {
      Nodes = new NodeView[SizeX, SizeY];

      for (int i = 0; i < SizeX; i++) {
        for (int j = 0; j < SizeY; j++) {
          Nodes[i, j] = NodeBuildingService.Build(ENode.Empty, i, j, StartingPoint);
        }
      }
    }

    [ContextMenu("Save")]
    void Save() {
      List<NodeModel> nodes = new List<NodeModel>();

      for (int i = 0; i < SizeX; i++) {
        for (int j = 0; j < SizeY; j++) {
          nodes.Add(Nodes[i, j].Model);
        }
      }
      
      GridState.Nodes = nodes;
#if UNITY_EDITOR
      EditorUtility.SetDirty(GridState);
#endif
    }
    
    void Load() {
      Nodes = new NodeView[SizeX, SizeY];

      var gridState = Levels.Count > 0 
        ? Levels[Random.Range(0, Levels.Count)]
        : GridState;
      
      foreach (NodeModel node in gridState.Nodes) {
        Nodes[node.X, node.Y] = NodeBuildingService.Build(node.ENode,node.X, node.Y, StartingPoint);
      }
    }

    public double Cost(Location a, Location b) {
      return 1;
    }

    public IEnumerable<Location> Neighbors(Location id) {
      foreach (var dir in DIRS) {
        Location next = new Location(id.x + dir.x, id.y + dir.y);
        if (IsInBounds(next) && IsPassable(next)) {
          yield return next;
        }
      }
    }

    public bool IsInBounds(Location id) {
      return 0 <= id.x && id.x < SizeX
                       && 0 <= id.y && id.y < SizeY;
    }
    
    public bool IsPassable(Location id)
    {
      return Nodes[id.x, id.y].Model.ENode == ENode.Road || Nodes[id.x, id.y].Model.ENode == ENode.Intersection;
    }

    public List<Vector3> GetWayPoints(List<Location> locations) {
      List<Vector3> wayPoints = new List<Vector3>();
      
      if (locations.Count < 2) {
        Debug.LogError("No locations");
        return wayPoints;
      }
      
      for (int i = 1; i < locations.Count; i++) {
        wayPoints.Add(GetWayPoint(locations[i - 1], locations[i], locations[i - 1]));
      }

      // wayPoints.Add(GetWayPoint(locations[0], locations[1], locations[0]));

      // for (int i = 1; i < locations.Count - 1; i++) {
      //   Location from = locations[i - 1];
      //   Location to = locations[i];
      //   Vector3 direction = new Vector3(to.x, 0, to.y) - new Vector3(from.x, 0, from.y);
      //   List<Vector3> wayPointsInLocation = Nodes[to.x, to.y].GetWayPoints(direction);
      //   wayPoints.AddRange(wayPointsInLocation);
      // }

      wayPoints.Add(GetWayPoint(locations[locations.Count - 2], locations[locations.Count - 1], locations[locations.Count - 1]));
      
      //Backwards direction

      wayPoints.Add(GetWayPoint(locations[locations.Count - 1], locations[locations.Count - 2], locations[locations.Count - 1]));

      // wayPoints.Add(GetWayPoint(locations[locations.Count - 1], locations[locations.Count - 2], locations[locations.Count - 1]));
      //
      // for (int i = locations.Count - 1; i > 1; i--) {
      //   Location from = locations[i];
      //   Location to = locations[i-1];
      //   Vector3 direction = new Vector3(to.x, 0, to.y) - new Vector3(from.x, 0, from.y);
      //   List<Vector3> wayPointsInLocation = Nodes[to.x, to.y].GetWayPoints(direction);
      //   wayPointsInLocation.Reverse();
      //   wayPoints.AddRange(wayPointsInLocation);
      // }
            
      for (int i = locations.Count - 1; i >= 1; i--) {
        wayPoints.Add(GetWayPoint(locations[i], locations[i - 1], locations[i - 1]));
      }
      
      // wayPoints.Add(GetWayPoint(locations[1], locations[0], locations[0]));
      
      return wayPoints;
    }

    Vector3 GetWayPoint(Location from, Location to, Location wayPointLocation) {
      Vector3 direction = new Vector3(to.x, 0, to.y) - new Vector3(from.x, 0, from.y);
      // Debug.DrawRay(new Vector3(from.x, 0, from.y), Vector3.up*100, Color.blue, 100);
      return Nodes[wayPointLocation.x, wayPointLocation.y].GetCenterWayPoint(direction);
    }
  }
}