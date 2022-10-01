using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Pathfinding {
  public class NGrid : MonoBehaviour {
    public NodeView[,] Nodes;
    public NodeBuildingService NodeBuildingService;
    public GridState GridState;
    public Transform StartingPoint;
    public GameObject EmptyPrefab;
    public int SizeX;
    public int SizeY;
    public bool NeedLoad;

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

      foreach (NodeModel node in GridState.Nodes) {
        Nodes[node.X, node.Y] = NodeBuildingService.Build(node.ENode,node.X, node.Y, StartingPoint);
      }
    }
  }
}