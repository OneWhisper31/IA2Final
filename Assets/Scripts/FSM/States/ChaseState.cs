using System.Collections.Generic;
using FSM;
using UnityEngine;

public class ChaseState : MonoBaseState {
    

    public override void UpdateLoop() {

    }

    public override IState ProcessInput() {

        return this;
    }
}