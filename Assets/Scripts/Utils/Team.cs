using UnityEngine;
using System;

[Flags]
public enum Team {
    None = 0b0000000,
    Loyal = 0b0000001,
    Traitor = 0b0000010,
    Agent  = 0b0000100,
    Real = Loyal | Traitor,
    AgentLoyal = Agent | Loyal,
    All = Loyal | Traitor | Agent
}

public static class TeamUtils {
  // Works with "None" as well
  public static bool HasFlag (this Team a, Team b) {
      return (a & b) == b;
  }
}
