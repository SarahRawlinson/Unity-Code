using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RPG.Core
{
    public interface IStat
    {
        float PercentLeft();
        float CurrentValue();
        float StartValue();
        event Action onStatChange;
    }
}
