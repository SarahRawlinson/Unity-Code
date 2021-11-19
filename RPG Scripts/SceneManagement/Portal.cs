using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.SceneManagement
{
    public enum PortalTypes { A, B, C, D, E, F, G };
    [System.Serializable]
    public class Portal { public Levels.GameLevels level; public PortalTypes portalType; }
}
