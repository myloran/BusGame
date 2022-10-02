using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
  [CreateAssetMenu(menuName = "Data/NodeBuildingService", fileName = "NodeBuildingService")]
  public class NodeBuildingService : ScriptableObject {
    public List<NodePrefab> Prefabs = new List<NodePrefab>();

    public NodeView Build(ENode eNode, int x, int y, Transform startingPoint) {
      for (int i = 0; i < Prefabs.Count; i++) {
        if (Prefabs[i].ENode == eNode) {
          GameObject obj = Instantiate(Prefabs[i].Prefab, new Vector3(x, 0, y), Quaternion.identity, startingPoint);
          var view = obj.GetComponent<NodeView>() ?? obj.AddComponent<NodeView>();
          view.Obj = obj;
          view.Model = new NodeModel(eNode, x, y);
          return view;
        }
      }

      return default;
    }
  }
}