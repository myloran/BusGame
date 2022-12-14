using System;
using DefaultNamespace.Events;
using DefaultNamespace.Pathfinding.States;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace DefaultNamespace.Economy {
  public class EconomyController : MonoBehaviour {
    public GameState GameState;
    public static GameState State;
    public int StartingMoney = 100;
    public int TimeToSpawnPassenger = 10;
    public int TimeToRemovePassenger = 10;
    public int TimeForBusToPayForGasoline = 10;
    public int TimeOfNoPassengerAppearingOnceCollected = 10;
    public int PassengerPercentToSpawn = 50;
    public int PassengerPercentToRemove = 50;
    public int PassengerMaxPercentToRemove = 50;
    public int MoneyPerCollectedPassenger = 1;
    public int BusCost = 1;
    public int GasolineCost = 1;
    
    //Add vehicle stats

    public int PassengerWinCondition = 50;
    public int MoneyLoseCondition = 0;
    
    public float Timer;
    public float Timer2;
    public float Timer3;
    
    public BusStationView[] BusStations;
    public bool IsStarted;

    public int MoneyDiff;
    public int PassengerDiff;

    public TMP_Text TMoney;
    public TMP_Text TPassengers;
    public TMP_Text TTime;
    public Text TMoney2;
    public Text TPassengers2;

    public int BusCount;
    public bool IsStartedForReal;
    public Canvas WinPopupCanvas;
    public Canvas LosePopupCanvas;
    public Text WinText;

    public PathBuilderState PathBuilderState;

    public void Init() {
      State = GameState;
      BusStations = FindObjectsOfType<BusStationView>();
      GameState.Money = StartingMoney;
      GameState.PassengersCollected = 0;
      MoneyDiff = 0;
      PassengerDiff = 0;
      BusCount = 0;
      IsStarted = true;
      EventController.BusUsed += BusUsed;
      EventController.BusBought += BusBought;
      EventController.LocationVisited += LocationVisited;
    }

    void BusUsed() {
      BusCount++;
      IsStartedForReal = true;
    }

    void LocationVisited(Location location) {
      foreach (BusStationView view in BusStations) {
        if (location != view.Model.Location) continue;
        
        int count = view.Model.PassengerCount;
        
        PassengerDiff += count;
        MoneyDiff += count * MoneyPerCollectedPassenger;

        GameState.PassengersCollected += count;
        GameState.Money += count * MoneyPerCollectedPassenger;

        view.Model.PassengerCount = 0;
        if (count > 0) view.Model.TimeBeforeNextSpawn = TimeOfNoPassengerAppearingOnceCollected;

        view.ClearPassengers();
        return;
      }
    }

    void BusBought() {
      if (GameState.Money - BusCost >= 0) {
        MoneyDiff -= BusCost;
        GameState.Money -= BusCost;
        EventController.BusBoughtConfirm();
      }
      //Add to ui
      //Allow to click it
    }

    public void ResetState() {
      MoneyDiff = 0;
      PassengerDiff = 0;
      GameState.Money = StartingMoney;
      GameState.PassengersCollected = 0;
      BusCount = 0;
      IsStarted = false;
      IsStartedForReal = false;
      EventController.BusUsed -= BusUsed;
      EventController.BusBought -= BusBought;
      EventController.LocationVisited -= LocationVisited;
    }

    void Update() {
      if (GameState.Money < 0) {
        GameState.Money = 0;
        // WinText.text = "Level failed!";
        LosePopupCanvas.enabled = true;
        PathBuilderState.ResetPath();
        // ResetState();
      }
      
      if (GameState.PassengersCollected > PassengerWinCondition) {
        // WinText.text = "Level completed!";
        WinPopupCanvas.enabled = true;
        PathBuilderState.ResetPath();
        // ResetState();
      }
      
      Timer += Time.deltaTime;
      Timer2 += Time.deltaTime;
      Timer3 += Time.deltaTime;

      foreach (BusStationView view in BusStations) {
        view.Model.TimeBeforeNextSpawn -= Time.deltaTime;
      }

      if (IsStartedForReal) {
        if (Timer > TimeToSpawnPassenger) {
          Timer = 0;
          MoneyDiff = 0;
          PassengerDiff = 0;
          SpawnPassengers();
          //check win condition
          //check lose condition
        }

        if (Timer2 > TimeToRemovePassenger) {
          RemovePassengers();
          Timer2 = 0;
        }

        if (Timer3 > TimeForBusToPayForGasoline) {
          PayForGasoline();
          Timer3 = 0;
        }
      }

      TTime.text = "Time: " + Timer.ToString("F");
      TMoney.text = "Money: " + GameState.Money + "(" + MoneyDiff + ")";
      TMoney2.text = GameState.Money.ToString();
      TPassengers.text = "Passengers collected: " + GameState.PassengersCollected + "(" + PassengerDiff + ")";
      TPassengers2.text = GameState.PassengersCollected.ToString();
    }

    void PayForGasoline() {
      MoneyDiff -= BusCount * GasolineCost;
      GameState.Money -= BusCount * GasolineCost;
      EventController.PayForGasoline();
    }

    void SpawnPassengers() {
      foreach (BusStationView view in BusStations) {
        if (view.Model.TimeBeforeNextSpawn > 0) continue;
        if (Random.Range(1, 100) <= PassengerPercentToSpawn) continue;

        view.SpawnPassenger();
        view.Model.PassengerCount++;
      }
    }
    
    void RemovePassengers() {
      foreach (BusStationView view in BusStations) {
        if (Random.Range(1, 100) <= PassengerPercentToRemove) continue;

        int passengersPercent = Random.Range(1, PassengerMaxPercentToRemove);
        var count = view.Model.PassengerCount * passengersPercent / 100;
        Debug.Log($"Removed: {count}");
        view.Model.PassengerCount -= count;
        view.ClearPassengers(count);
      }
    }
  }
}