using System;
using System.Collections.Generic;

namespace DefaultNamespace.Economy {
  [Serializable]
  public class GameState {
    public int Money;
    public int PassengersCollected;
    public List<BusStationModel> BusStations = new List<BusStationModel>();
    public List<BusModel> Buses = new List<BusModel>();
  }
}