using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Guard
{
    public class GuardIdleState : MonoBaseState
    {
        [SerializeField] float speed;
        [SerializeField] Transform[] waypoints = { };

        int index;
        bool isWaiting;
        
        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isEscorting", false);
            animator.speed = 1;

        }

        public override void UpdateLoop(){
            if (isWaiting)
                return;
            animator.SetBool("isEscorting", true);

            Vector3 dir = waypoints[index].position - transform.position;
            dir.y = 0;

            if (dir.magnitude <= 1f)
            {
                StartCoroutine(WaitAction());
                return;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.forward = dir;
        }
        IEnumerator WaitAction()
        {
            if (index + 1 >= waypoints.Length)
                index = 0;
            else
                index++;
            animator.SetBool("isEscorting", false);

            yield return new WaitForSecondsRealtime(1);
            isWaiting = false;

        }


        public override IState ProcessInput()
        {//IA2-LINQ
            var query = myCharacter.circleQuery.Query().Select(x => (Character)x)
                .Where(x=>(transform.position-x.transform.position).magnitude<=3f);

            if (query.Where(x => x.GetComponent<Looter.Looter>() != null).Count() >= 1
                && Transitions.ContainsKey("OnAttack"))
                return Transitions["OnAttack"];

            if (query.Where(x => x.GetComponent<Slave.Slave>()!=null).Count() >= 1 
                && Transitions.ContainsKey("OnEscort"))
                return Transitions["OnEscort"];

            return this;
        }
    }

}
