using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Guard
{
    public class GuardIdleState : MonoBaseState
    {
        public Slave.Slave slave;//handled by Slave
        
        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isEscorting", false);

            if (slave!=null && slave.IsDead)
                slave = null;
        }

        public override void UpdateLoop(){}

        public override IState ProcessInput()
        {//IA2-LINQ

            if (slave!=null&& Transitions.ContainsKey("OnEscort"))
                return Transitions["OnEscort"];
            else if(myCharacter.circleQuery.Query().Where(x => x.GetType() == typeof(Looter.Looter)).ToArray().Length > 0 
                && Transitions.ContainsKey("OnAttack"))
                return Transitions["OnAttack"];

            return this;
        }
    }

}
