using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Slave
{
    public class SlaveFleeState : MonoBaseState
    {
        [SerializeField] Transform house;

        [SerializeField] float speed, waitTime;

        bool hasArrive, hasEmptied;

        CapsuleCollider capsule;

        List<Transform> path = new List<Transform>();
        Transform target;
        bool isWaitingForPath;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isMining", false);
            hasArrive = false;
            hasEmptied = false;

            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();
        }

        public override void UpdateLoop()
        {
            if (hasArrive)
                return;

            if (transform.position.CanPassThrough(house.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = house;

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

                if ((transform.position - house.position).magnitude < 0.5f)
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
            var nodes = myCharacter.GenerateStartNEnd(transform, house);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                path.Clear();
                path = x.Select(x => x.transform).PrependAppend(transform, house).ToList();//IA2-LINQ
                target = path.First();
                isWaitingForPath = false;
            }
            ));
        }
        public IEnumerator WaitAction()
        {
            yield return new WaitForSecondsRealtime(waitTime);
            animator.speed = 1;
            hasEmptied = true;
        }
        public override IState ProcessInput()
        {
            if (hasEmptied && Transitions.ContainsKey("OnEmpty"))
            {
                animator.SetBool("isMining", false);
                return Transitions["OnEmpty"];
            }

            return this;
        }
    }

}
