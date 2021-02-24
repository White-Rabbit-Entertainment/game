using UnityEngine;
using System;

[System.Serializable]
[Flags]
public enum Team {
    None      = 0b_0000,
    Robber    = 0b_0001,
    Seeker    = 0b_0010,
    Agent     = 0b_0100,
    Real      = Seeker | Robber,
    All       = Seeker | Robber | Agent
}
