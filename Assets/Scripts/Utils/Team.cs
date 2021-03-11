using UnityEngine;
using System;

[Flags]
public enum Team {
    None            = 0b0000000,
    NonCaptainLoyal = 0b0000001,
    Captain         = 0b0000010,
    Traitor         = 0b0000100,
    Agent           = 0b0001000,
    Ghost           = 0b0010000,
    Loyal           = NonCaptainLoyal | Captain,
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
