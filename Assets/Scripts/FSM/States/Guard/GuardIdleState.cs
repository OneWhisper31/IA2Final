using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Guard
{
    public class GuardIdleState : MonoBaseState
    {
        public override IState ProcessInput()
        {
            return this;
        }

        public override void UpdateLoop()
        {
            Debug.Log("Guard");
        }
    }

}
