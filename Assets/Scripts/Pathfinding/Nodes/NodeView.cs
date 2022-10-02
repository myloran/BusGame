using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
  public class NodeView : MonoBehaviour {
    public GameObject Obj;
    public NodeModel Model;

    public Transform F;
    public Transform R;
    public Transform B;
    public Transform L;
    
    public Transform FFL;
    public Transform FFR;
    public Transform RRF;
    public Transform RRB;
    public Transform BBR;
    public Transform BBL;
    public Transform LLB;
    public Transform LLF;

    public Vector3 GetCenterWayPoint(Vector3 direction) {
      if (direction == Vector3.forward) return L.position;
      if (direction == Vector3.back) return R.position;
      if (direction == Vector3.left) return B.position;
      if (direction == Vector3.right) return F.position;

      Debug.LogError("Direction not handled");
      return default;
    }

    public List<Vector3> GetWayPoints(Vector3 direction) {
      if (direction == Vector3.forward) return new List<Vector3>{ FFL.position, BBL.position };
      if (direction == Vector3.back) return new List<Vector3>{ FFR.position, BBR.position };
      if (direction == Vector3.left) return new List<Vector3>{ LLB.position, RRB.position };
      if (direction == Vector3.right) return new List<Vector3>{ LLF.position, RRF.position };
      
      Debug.LogError("Direction not handled");
      return default;
    }
  }
}