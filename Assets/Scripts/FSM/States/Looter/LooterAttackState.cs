using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM.Looter
{
    public class LooterAttackState : MonoBaseState
    {
        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
        }
        public override IState ProcessInput()
        {
            return this;
        }

        public override void UpdateLoop()
        {
            Debug.Log("attack");
        }
    }

}
