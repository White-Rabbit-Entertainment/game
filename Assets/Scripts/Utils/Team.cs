using UnityEngine;
using System;

[Flags]
public enum Team {
    Robber = 0b0000001,
    Seeker = 0b0000010,
    Agent  = 0b0000100,
    Real = Robber | Seeker,
    AgentRobber = Agent | Robber,
    All = Robber | Seeker | Agent
}

public static class TeamUtils {
  // Works with "None" as well
  public static bool HasFlag (this Team a, Team b) {
      return (a & b) == b;
  }
}
