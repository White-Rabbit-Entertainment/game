using UnityEngine;
using System;

// Flag enum for teams, this allows boolean operation between teams (eg reals
// which holds players which are not agents).
[Flags]
public enum Team {
    None            = 0b0000000,
    Loyal           = 0b0000010,
    Traitor         = 0b0000100,
    Ghost           = 0b0001000,
    Agent           = 0b0010000,
    Real            = Loyal | Traitor,
    AgentLoyal      = Agent | Loyal,
    All             = Loyal | Traitor | Agent
}

public static class TeamUtils {
  // To check if a team contains the flag for a provided team. Eg to check if a
  // the Real team contains the Loyal flag.
  public static bool HasFlag (this Team a, Team b) {
      return (a & b) == b;
  }
}
