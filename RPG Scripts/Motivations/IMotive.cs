using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Motivation
{
    public interface IMotive
    {        
        void Activate();
        void Cancel();
    }
}
