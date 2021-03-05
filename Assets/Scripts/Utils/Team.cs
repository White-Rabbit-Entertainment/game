using UnityEngine;
using System;

[Flags]
public enum Team {
    Robber,
    Seeker,
    Agent,
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
