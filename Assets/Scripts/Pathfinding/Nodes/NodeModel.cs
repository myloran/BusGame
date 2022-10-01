using System;

namespace Pathfinding {
  [Serializable]
  public class NodeModel {
    public ENode ENode;
    public int X;
    public int Y;

    public NodeModel(ENode eNode, int x, int y) {
      ENode = eNode;
      X = x;
      Y = y;
    }
  }
}