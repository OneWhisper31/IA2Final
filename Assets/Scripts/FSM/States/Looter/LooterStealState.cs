using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Looter
{
    public class LooterStealState : MonoBaseState
    {
        [SerializeField] Transform medievalHouse;

        [SerializeField] float speed;
        [SerializeField] int maxCapacity=100;

        int loot;
        bool isStealing;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
            isStealing = false;
            loot = 0;
            foreach (var item in Transitions)
            {
                Debug.Log(item.Key +": " +item.Value);
            }
        }

        public override void UpdateLoop()
        {
            Vector3 dir = medievalHouse.position- transform.position;
            dir.y = 0;

            if (dir.magnitude < 0.5f)
            {
                if (!isStealing)
                {
                    animator.SetBool("isStealing", true);
                    isStealing = true;
                }
            }
            else
            {
                if (isStealing)
                {
                    animator.SetBool("isStealing", false);
                    isStealing = false;
                }

                transform.position += dir.normalized * speed * Time.deltaTime;
                transform.forward = dir;
            }
        }
        public void StealAction()
        {
            if (loot == maxCapacity)
                return;
            loot += 25;
            if(loot==maxCapacity)
                animator.SetBool("isStealing", false);
        }
        public override IState ProcessInput()
        {
            if (loot >= maxCapacity && Transitions.ContainsKey("OnFull"))
                return Transitions["OnFull"];
            else if(myCharacter.circleQuery.Query().Select(x=>(Character)x)
                    .Where(x=>x.gameObject!=this.gameObject).Any(x=>x!=null)//IA2-LINQ
                        && Transitions.ContainsKey("OnAttack"))
                return Transitions["OnAttack"];

            return this;
        }
    }

}

