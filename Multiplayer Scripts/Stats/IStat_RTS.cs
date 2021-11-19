using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace MultiplayerRTS.Stats
{
    public interface IStat_RTS
    {
        int GetStatValue();
        int GetStartStatValue();
        float GetPercentageStatRemaining();
        event Action<int, int> ClientOnUpdateToStats;
    }
}
