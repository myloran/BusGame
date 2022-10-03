using System;
using DefaultNamespace.Events;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace.Economy {
  public class EconomyController : MonoBehaviour {
    public GameState GameState;
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
    
    public BusStationView[] BusStations;
    public bool IsStarted;

    public int MoneyDiff;
    public int PassengerDiff;

    public TMP_Text TMoney;
    public TMP_Text TPassengers;
    public TMP_Text TTime;

    public int BusCount;

    public void Init() {
      BusStations = FindObjectsOfType<BusStationView>();
      GameState.Money = StartingMoney;
      GameState.PassengersCollected = 0;
      MoneyDiff = 0;
      PassengerDiff = 0;
      BusCount = 0;
      IsStarted = true;
      EventController.PassengerCollected += PassengerCollected;
      EventController.BusBought += BusBought;
    }

    void BusBought() {
      MoneyDiff -= BusCost;
      GameState.Money -= BusCost;
      BusCount++;
      //Add to ui
      //Allow to click it
    }

    void PassengerCollected(int count) {
      PassengerDiff += count;
      MoneyDiff += count * MoneyPerCollectedPassenger;

      GameState.PassengersCollected += count;
      GameState.Money += count * MoneyPerCollectedPassenger;
      
      // view.Model.TimeBeforeNextSpawn = TimeOfNoPassengerAppearingOnceCollected;
    }

    public void ResetState() {
      MoneyDiff = 0;
      PassengerDiff = 0;
      GameState.Money = StartingMoney;
      GameState.PassengersCollected = 0;
      BusCount = 0;
      IsStarted = false;
    }

    void Update() {
      Timer += Time.deltaTime;

      foreach (BusStationView view in BusStations) {
        view.Model.TimeBeforeNextSpawn -= Time.deltaTime;
      }
      
      if (Timer > TimeToSpawnPassenger) {
        Timer = 0;
        MoneyDiff = 0;
        PassengerDiff = 0;
        RemovePassengers();
        SpawnPassengers();
        PayForGasoline();
        //check win condition
        //check lose condition
      }
      
      TTime.text = "Time: " + Timer.ToString("F");
      TMoney.text = "Money: " + GameState.Money + "(" + MoneyDiff + ")";
      TPassengers.text = "Passengers collected: " + GameState.PassengersCollected + "(" + PassengerDiff + ")";
    }

    void PayForGasoline() {
      MoneyDiff -= BusCount * GasolineCost;
      GameState.Money -= BusCount * GasolineCost;
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
        view.Model.PassengerCount -= view.Model.PassengerCount * passengersPercent / 100;
        Debug.Log($"Removed: {passengersPercent}");
      }
    }
  }
}