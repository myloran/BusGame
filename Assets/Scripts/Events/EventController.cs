using System;

namespace DefaultNamespace.Events {
  public class EventController {
    public static Action<int> PassengerCollected = (amount) => {};
    public static Action BusBought = () => { };
  }
}