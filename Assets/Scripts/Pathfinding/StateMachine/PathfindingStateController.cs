using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public class PathfindingStateController : MonoBehaviour {
    public PathfindingStateMachine PathfindingStateMachine;
    public TMP_Dropdown DPathfindingState;

    public void Init() {
      var names = Enum.GetNames(typeof(EPathfinderState));
      var options = names.Select(n => new TMP_Dropdown.OptionData(n));
      DPathfindingState.options.Clear();
      DPathfindingState.options.AddRange(options);
      if (names.Any()) DPathfindingState.captionText.text = names[0];
      
      DPathfindingState.onValueChanged.AddListener(OnPathfindingStateChanged);
    }

    void OnPathfindingStateChanged(int index) {
      PathfindingStateMachine.ChangeStateTo((EPathfinderState)index);
    }
  }
}