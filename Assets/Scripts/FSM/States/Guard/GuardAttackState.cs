using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Guard
{
    public class GuardAttackState : MonoBaseState
    {
        public override IState ProcessInput()
        {
            return this;
        }

        public override void UpdateLoop()
        {

        }
    }

}
