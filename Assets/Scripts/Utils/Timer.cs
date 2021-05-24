using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// Class to store static instances of timers.
public static class TimerUtils {

  // List of all timers
  public static List<Timer> timers = new List<Timer>();

  // Round timers - If ends then the round is over and the traitor wins
  public static Timer roundTimer = new Timer(420, "roundTimer");
  
  // Vote timer - Started with vote, if ended then the vote is stopped with no
  // result
  public static Timer voteTimer = new Timer(30, "voteTimer");

  // Sabotage timer - The timer for an active sabotage, if ends traitor wins
  public static Timer sabotageTimer = new Timer(30, "sabotageTimer");

  // Traitor sabotage timer - The timer for before players are notified about a
  // sabotage.
  public static Timer traitorSabotageTimer = new Timer(5, "traitorSabotageTimer");
}

public class Timer {
  // If the timer has started
  bool started = false;
  // When the timer was started
  double startTime;
  // How long the timer should last in seconds
  double length;
  // The id of the timer, used to get timer with Timer.Get
  public string id;

  // Getter for static timer instances 
  public static Timer roundTimer {
    get {return TimerUtils.roundTimer;}
  }
  public static Timer voteTimer {
    get {return TimerUtils.voteTimer;}
  }
  public static Timer sabotageTimer {
    get {return TimerUtils.sabotageTimer;}
  }
  public static Timer traitorSabotageTimer{
    get {return TimerUtils.traitorSabotageTimer;}
  }

  // Init timer with lenght and an id to reference the timer
  public Timer(double length, string id) {
    this.length = length;
    TimerUtils.timers.Add(this);
    this.id = id;
  }

  // Static method to get a timer from a timer id
  public static Timer Get(string id) {
    foreach (Timer timer in TimerUtils.timers) {
      if (timer.id == id) return timer;
    }
    return default;
  }

  // Start a timer and set the start time
  public void Start(double startTime) {
    started = true;
    this.startTime = startTime;
  }

  // End a timer
  public void End() {
    started = false;
  }

  // Check if a timer has started (is not ended)
  public bool IsStarted() {
    return started;
  }
   
  // Return time remaining in seconds
  public double TimeRemaining() {
    return length - (PhotonNetwork.Time - startTime);
  }

  // Check if a timer has been completed (if it has ended then it is not
  // completed).
  public bool IsComplete() {
    return started && TimeRemaining() <= 0;
  }
   
  // Format time to a string of format mm:ss
  public string FormatTime() {
    int secondsLeft = (int)(TimeRemaining());
    int minutes = secondsLeft / 60;
    int seconds = secondsLeft % 60;
    string secondsStr = seconds.ToString();
    string minutesStr = minutes.ToString();
    if (minutes < 10) minutesStr = "0" + minutesStr;
    if (seconds < 10) secondsStr = "0" + secondsStr;
    return minutesStr + ":" + secondsStr;
  }
}
