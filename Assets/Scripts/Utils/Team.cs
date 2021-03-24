using UnityEngine;
using System;

[Flags]
public enum Team {
    None            = 0b0000000,
    Traitor         = 0b0000100,
    Agent           = 0b0001000,
    Ghost           = 0b0010000,
    Loyal           = 0b0100000,
    Real            = Loyal | Traitor,
    AgentLoyal      = Agent | Loyal,
    All             = Loyal | Traitor | Agent
}

public static class TeamUtils {
  // Works with "None" as well
  public static bool HasFlag (this Team a, Team b) {
      return (a & b) == b;
  }
}
