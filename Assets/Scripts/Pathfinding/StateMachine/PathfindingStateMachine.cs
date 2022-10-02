using System;
using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public class PathfindingStateMachine : MonoBehaviour {
    public BaseState StartingState;
    public GridBuilderState GridBuilderState;
    public PathBuilderState PathBuilderState;

    IState currentState;

    public void ChangeStateTo(EPathfinderState state) {
      switch (state) {
        case EPathfinderState.GridBuilder:
          ChangeStateTo(GridBuilderState);
          break;
        case EPathfinderState.PathBuilder:
          ChangeStateTo(PathBuilderState);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    void ChangeStateTo(IState to) {
      currentState?.OnExit();
      currentState = to;
      currentState.OnEnter();
    }

    public void Init() {
      GridBuilderState.Init();
      PathBuilderState.Init();
      ChangeStateTo(StartingState);
    }

    public void Update() {
      currentState?.OnUpdate();
    }
  }
}