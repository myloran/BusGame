using System;

namespace DefaultNamespace.Events {
  public class EventController {
    public static Action<Location> LocationVisited = l => { };
    public static Action<int> PassengerCollected = amount => {};
    public static Action BusBought = () => { };
    public static Action BusBoughtConfirm = () => { };
    public static Action BusUsed = () => { };
    public static Action PayForGasoline = () => { };
  }
}