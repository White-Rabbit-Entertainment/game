using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public static class TimerUtils {
  // List of all timers
  public static List<Timer> timers = new List<Timer>();
  public static Timer roundTimer = new Timer(420, "roundTimer");
  public static Timer voteTimer = new Timer(30, "voteTimer");
  public static Timer sabotageTimer = new Timer(30, "sabotageTimer");
  public static Timer traitorSabotageTimer = new Timer(5, "traitorSabotageTimer");
}

public class Timer {
  bool started = false;
  double startTime;
  double length;
  public string id;

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

  public Timer(double length, string id) {
    this.length = length;
    Debug.Log(TimerUtils.timers);
    TimerUtils.timers.Add(this);
    this.id = id;
  }

  public static Timer Get(string id) {
    foreach (Timer timer in TimerUtils.timers) {
      if (timer.id == id) return timer;
    }
    return default;
  }

  public void Start(double startTime) {
    started = true;
    this.startTime = startTime;
  }

  public void End() {
    started = false;
  }

  public bool IsStarted() {
    return started;
  }
    
  public double TimeRemaining() {
    return length - (PhotonNetwork.Time - startTime);
  }

  public bool IsComplete() {
    return started && TimeRemaining() <= 0;
  }
    
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
