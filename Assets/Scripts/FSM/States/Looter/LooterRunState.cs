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

        CapsuleCollider capsule;

        List<Transform> path = new List<Transform>();
        Transform target;
        bool isWaitingForPath;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
            animator.speed = 1;
            hasArrive = false;
            canSteal = false;

            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();
        }

        public override void UpdateLoop()
        {
            if (hasArrive)
                return;

            if (transform.position.CanPassThrough(looterHouse.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = looterHouse;

                if (path.Count > 0)
                    path.Clear();
            }
            else
            {
                if (!isWaitingForPath && path.Count <= 0)
                    GeneratePath();

                if (path.Count > 0)
                    target = path.First();
                else
                    target = null;
            }

            if (target != null)
            {
                Vector3 dir = target.position - transform.position;
                dir.y = 0;

                if ((transform.position - looterHouse.position).magnitude < 0.5f)
                {
                    animator.speed = 0;
                    hasArrive = true;
                    StartCoroutine(WaitAction());
                }
                else
                {
                    if (dir.magnitude < 0.5f && path.Count > 0)
                    {//IA2-LINQ
                        path = path.Skip(1).ToList();//next Target
                    }

                    transform.position += dir.normalized * speed * Time.deltaTime;
                    transform.forward = dir;
                }
            }
        }
        public void GeneratePath()
        {//IA2-LINQ

            isWaitingForPath = true;
            var nodes = myCharacter.GenerateStartNEnd(transform, looterHouse);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                path.Clear();
                path = x.Select(x => x.transform).PrependAppend(transform, looterHouse).ToList();//IA2-LINQ
                target = path.First();
                isWaitingForPath = false;
            }
            ));
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
