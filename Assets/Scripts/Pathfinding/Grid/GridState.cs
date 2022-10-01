using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
  [CreateAssetMenu(menuName = "Data/GridState", fileName = "GridState")]
  public class GridState : ScriptableObject {
    public List<NodeModel> Nodes = new List<NodeModel>(); 
  }
}