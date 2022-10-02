using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public abstract class BaseState : MonoBehaviour, IState {
    public virtual void Init() { }
    public virtual void OnEnter() { }
    public virtual void OnUpdate() { }
    public virtual void OnExit() { }
  }
}