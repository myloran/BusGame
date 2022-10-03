using System;

namespace DefaultNamespace.Economy {
  [Serializable]
  public class BusStationModel {
    public Location Location;
    public int PassengerCount;
    public float TimeBeforeNextSpawn;
  }
}