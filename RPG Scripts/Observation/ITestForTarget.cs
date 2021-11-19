using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Observation
{
    public interface ITestForTarget
    {
        (bool, GameObject) TestForTarget(Collider collider, List<GameObject> gameObjects);
    }
}
