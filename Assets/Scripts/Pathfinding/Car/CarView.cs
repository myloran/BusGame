using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Pathfinding.States {
  public class CarView : MonoBehaviour {
    public List<Vector3> WayPoints;
    public Vector3 Current;
    public Vector3 Next;
    public int CurrentIndex;
    public bool IsMovingToTheEnd;

    public BoxCollider ColliderInFront;
    public float Speed = 1;

    public void SetGoal(List<Vector3> wayPoints) {
      WayPoints = wayPoints;
      IsMovingToTheEnd = false;
      
      if (WayPoints.Any()) {
        CurrentIndex = 0;//WayPoints.Count - 1;
        Current = WayPoints[CurrentIndex];
      }
    }
    
    public void Update() {
      if (!WayPoints.Any()) return;
      
      (Current, CurrentIndex) = CheckIfCloseToNextLocation();

      // Vector3 goal = GetGoal();
      // if (IsCloseToPoint(goal)) {
        // IsMovingToTheEnd = !IsMovingToTheEnd; //needed if we want to change direction
      // }
      
      (Next, _) = CalculateNextLocation();

      DrawBox(ColliderInFront.transform.position + ColliderInFront.center, Quaternion.identity, Vector3.one / 20, Color.blue);
      if (Physics.CheckBox(ColliderInFront.transform.position + ColliderInFront.center, Vector3.one / 20, Quaternion.identity, LayerMask.GetMask("Bus"))) return;
      

      Vector3 direction = Next - Current;
      transform.position += Speed * Time.deltaTime * direction;
      transform.rotation = Quaternion.LookRotation(direction);
      
      // float maxDistance = Vector3.Distance(transform.position, goal);
      // transform.position += Speed * Time.deltaTime * Vector3.ClampMagnitude(direction, maxDistance);
    }

    bool IsCloseToPoint(Vector3 target) {
      float distance = Vector3.Distance(transform.position, target);
      return distance < 0.05;
      
      // Vector3 direction = transform.position - target;
      // Vector3 abs = new Vector3(direction);
      // return Mathf.Abs(direction).sqrMagnitude < 0.05;
    }

    Vector3 GetGoal() {
      int index = WayPoints.Count - 1;//IsMovingToTheEnd ? WayPoints.Count - 1 : 0; //needed if we want to change direction
      return new Vector3(WayPoints[index].x, 0, WayPoints[index].y);
    }

    (Vector3, int) CheckIfCloseToNextLocation() {
      (Vector3 next, int index) = CalculateNextLocation();
      
      return IsCloseToPoint(next) 
        ? (next, index) 
        : (Current, currentIndex: CurrentIndex);

      // Location min = Path.First();
      // float minDistance = float.MaxValue;
      // int index = 0;
      //
      // for (var i = 0; i < Path.Count; i++) {
      //   Location location = Path[i];
      //   float distance = (new Vector3(location.x, 0, location.y) - transform.position).sqrMagnitude;
      //   if (distance < minDistance) {
      //     minDistance = distance;
      //     min = location;
      //     index = i;
      //   }
      // }
      //
      // return (min, index);
    }
    
    (Vector3, int) CalculateNextLocation() {
      if (IsMovingToTheEnd) {
        var indexToEnd = (CurrentIndex + 1) % WayPoints.Count - 1;// Mathf.Min(currentIndex + 1, WayPoints.Count - 1);
        return (WayPoints[indexToEnd], indexToEnd);
      }

      int indexToStart = CurrentIndex - 1 < 0 ? WayPoints.Count - 1 : CurrentIndex - 1;//Mathf.Max(currentIndex - 1, 0);
      return (WayPoints[indexToStart], indexToStart);
    }
    
    public void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c)
    {
      // create matrix
      Matrix4x4 m = new Matrix4x4();
      m.SetTRS(pos, rot, scale);
 
      var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
      var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
      var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
      var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
 
      var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
      var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
      var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
      var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
 
      Debug.DrawLine(point1, point2, c);
      Debug.DrawLine(point2, point3, c);
      Debug.DrawLine(point3, point4, c);
      Debug.DrawLine(point4, point1, c);
 
      Debug.DrawLine(point5, point6, c);
      Debug.DrawLine(point6, point7, c);
      Debug.DrawLine(point7, point8, c);
      Debug.DrawLine(point8, point5, c);
 
      Debug.DrawLine(point1, point5, c);
      Debug.DrawLine(point2, point6, c);
      Debug.DrawLine(point3, point7, c);
      Debug.DrawLine(point4, point8, c);
 
      // optional axis display
      // Debug.DrawRay(m.GetPosition(), m.GetForward(), Color.magenta);
      // Debug.DrawRay(m.GetPosition(), m.GetUp(), Color.yellow);
      // Debug.DrawRay(m.GetPosition(), m.GetRight(), Color.red);
    }
  }
}