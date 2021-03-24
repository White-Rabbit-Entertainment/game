﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum Timer {
  RoundTimer,
}

public static class TimerUtils
{
    // Start the timer for the game, but assigning the start time and round length (which all clients use)
    public static void Start(this Timer timer, double roundLength) {
      NetworkManager.instance.SetRoomProperty(timer.ToString() + "length", roundLength);
      NetworkManager.instance.SetRoomProperty(timer.ToString() + "start", PhotonNetwork.Time);
      NetworkManager.instance.SetRoomProperty(timer.ToString() + "started", true);
    }

    public static void End(this Timer timer) {
      NetworkManager.instance.SetRoomProperty(timer.ToString() + "started", false);
    }
    
    public static bool IsStarted(this Timer timer) {
      return NetworkManager.instance.RoomPropertyIs<bool>(timer.ToString() + "started", true);
    }
    
    // Returns round time remaining (or 0 if not started)
    public static double TimeRemaining(this Timer timer) {
      return NetworkManager.instance.GetRoomProperty<double>(timer.ToString() + "length", 0f) - (PhotonNetwork.Time - NetworkManager.instance.GetRoomProperty<double>(timer.ToString() + "start", 0f));
    }

    public static bool IsCompleted(this Timer timer) {
      return timer.IsStarted() && timer.IsCompleted();
    }
}
