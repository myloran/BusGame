using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.Economy {
  public class BusStationView : MonoBehaviour {
    public BusStationModel Model;
    // public Bounds SpawnArea;
    public GameObject PassengerPrefab;
    public List<GameObject> Passengers;
    public GameObject TextPrefab;

    const int MaxAttempts = 10;
    
    public void SpawnPassenger() {
      for (int i = 0; i < MaxAttempts; i++) {
        Vector3 position = new Vector3(Random.Range(-4.5f, 4.5f), 0, Random.Range(-4.5f, 4.5f));
        DrawBox(position, Quaternion.identity, Vector3.one / 20, Color.blue);
        if (Physics.CheckBox(position, Vector3.one / 20, Quaternion.identity, LayerMask.GetMask("Passenger"))) continue;
        
        GameObject passenger = Instantiate(PassengerPrefab, position, quaternion.identity);
        passenger.transform.SetParent(transform, true);
        passenger.transform.localPosition = position;
        Passengers.Add(passenger);
        break;
      }
    }

    public void ClearPassengers() {
      for (int i = Passengers.Count - 1; i >= 0; i--) {
        GameObject text = Instantiate(TextPrefab, Passengers[i].transform.position, quaternion.identity);
        text.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);
        Destroy(Passengers[i]);
        Passengers.RemoveAt(i);
      }
      // for (var i = 0; i < Passengers.Count; i++) {
      //   var passenger = Passengers[i];
      //   Instantiate(TextPrefab, passenger.transform.position, quaternion.identity);
      //   Destroy(passenger);
      //   Passengers.RemoveAt(i);
      // }
    }
    
    public void ClearPassengers(int count) {
      int max = Mathf.Min(count, Passengers.Count);
      
      for (int i = max - 1; i >= 0; i--) {
        GameObject text = Instantiate(TextPrefab, Passengers[i].transform.position, quaternion.identity);
        text.transform.rotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);
        Destroy(Passengers[i]);
        Passengers.RemoveAt(i);
      }
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