using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Looter
{
    public class LooterAttackState : MonoBaseState
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
