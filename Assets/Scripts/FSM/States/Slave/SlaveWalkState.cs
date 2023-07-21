using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Slave
{
    public class SlaveWalkState : MonoBaseState
    {
        public override IState ProcessInput()
        {
            return this;
        }

        public override void UpdateLoop()
        {
            Debug.Log("Slave");
        }
    }

}
