using System;

namespace Lotus.GameModes;

public enum BlockableGameActions
{
    Nothing = 0,
    ReportBody = 1,
    CallMeeting = 2,
    KillPlayers = 4,
    CallSabotage = 8,
    CloseDoors = 16,
    EnterVent = 32
}