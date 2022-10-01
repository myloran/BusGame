using Pathfinding;
using UnityEngine;

namespace DefaultNamespace {
  public class GameEntryPoint : MonoBehaviour {
    public NGrid Grid;
    public GridBuilder GridBuilder;
    
    void Start() {
      Grid.Init();
      GridBuilder.Init();
    }
  }
}