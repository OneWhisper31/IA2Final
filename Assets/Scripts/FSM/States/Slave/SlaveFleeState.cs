using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Slave
{
    public class SlaveFleeState : MonoBaseState
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
