using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Looter 
{ 
public class LooterRunState : MonoBaseState
    {
        [SerializeField] Transform looterHouse;

        [SerializeField] float speed, waitTime;

        bool hasArrive, canSteal;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
            hasArrive = false;
            canSteal = false;
        }

        public override void UpdateLoop()
        {
            if (hasArrive)
                return;

            Vector3 dir = looterHouse.position - transform.position;
            dir.y = 0;

            if (dir.magnitude < 0.5f)
            {
                animator.speed = 0;
                hasArrive = true;
                StartCoroutine(WaitAction());
            }

                transform.position += dir.normalized * speed * Time.deltaTime;
                transform.forward = dir;
        }
        public IEnumerator WaitAction()
        {
            yield return new WaitForSecondsRealtime(waitTime);
            animator.speed = 1;
            hasArrive = true;
            canSteal = true;
        }
        public override IState ProcessInput()
        {
            if (canSteal && Transitions.ContainsKey("OnEmpty"))
            {
                animator.SetBool("isStealing", false);
                return Transitions["OnEmpty"];
            }
            else if (myCharacter.circleQuery.Query().Select(x => (Character)x)
                    .Where(x => x.gameObject != this.gameObject).Any(x => x != null)//IA2-LINQ
                        && Transitions.ContainsKey("OnAttack"))
                return Transitions["OnAttack"];

            return this;
        }
    }

}
