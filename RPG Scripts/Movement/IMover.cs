using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Movement
{
    public interface IMover
    {
        bool HasReachedDestination();
        bool HasReachedDestination(float stopRange);
        float GetStoppingRange();
        void MoveTo(Vector3 destination, float speed, float stopRange);
        void StartMoveAction(Vector3 destination, float speed);
        void StartMoveAction(Vector3 destination);


    }
}
