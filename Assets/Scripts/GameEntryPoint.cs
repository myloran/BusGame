using DefaultNamespace.Pathfinding.States;
using Pathfinding;
using UnityEngine;

namespace DefaultNamespace {
  public class GameEntryPoint : MonoBehaviour {
    public NGrid Grid;
    public PathfindingStateMachine PathfindingStateMachine;
    public PathfindingStateController PathfindingStateController;
    
    void Start() {
      Grid.Init();
      PathfindingStateMachine.Init();
      PathfindingStateController.Init();
    }
  }
}