namespace DefaultNamespace.Pathfinding.States {
  public interface IState {
    void Init();
    void OnEnter();
    void OnUpdate();
    void OnExit();
  }
}