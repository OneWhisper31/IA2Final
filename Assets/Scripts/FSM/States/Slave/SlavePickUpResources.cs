using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Slave
{
    public class SlavePickUpResources : MonoBaseState
    {
        [SerializeField]Transform gold;
        Transform guard;
        CapsuleCollider capsule;

        List<Transform> path = new List<Transform>();
        Transform target;

        [SerializeField] float speed;
        [SerializeField]int maxCapacity = 100;

        bool isWaitingForPath;
        bool isMining;
        int loot;


        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isMining", false);

            loot = 0;
            isMining = false;

            guard = GetComponent<SlaveWalkState>().guard;

            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();
        }

        public override void UpdateLoop()
        {
            if (transform.position.CanPassThrough(gold.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = gold;

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

                if ((transform.position - gold.position).magnitude < 0.5f)
                {
                    if (!isMining)
                    {
                        animator.SetBool("isMining", true);
                        isMining = true;
                    }
                }
                else
                {
                    if (dir.magnitude < 0.5f && path.Count > 0)
                    {//IA2-LINQ
                        path = path.Skip(1).ToList();//next Target
                    }

                    if (isMining)
                    {
                        animator.SetBool("isMining", false);
                        isMining = false;
                    }

                    transform.position += dir.normalized * speed * Time.deltaTime;
                    transform.forward = dir;
                }
            }
        }

        public void GeneratePath()
        {//IA2-LINQ

            isWaitingForPath = true;
            var nodes = myCharacter.GenerateStartNEnd(transform, gold);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                path.Clear();
                path = x.Select(x => x.transform).PrependAppend(transform, gold).ToList();//IA2-LINQ
                target = path.First();
                isWaitingForPath = false;
            }
            ));
        }
        public void SlaveAction()
        {
            loot += 25;
            if (loot == maxCapacity)
                animator.SetBool("isMining", false);
        }
        public override IState ProcessInput()
        {
            if ((loot >= maxCapacity || (transform.position-guard.transform.position).magnitude>5.5f)
                && Transitions.ContainsKey("OnBack"))
                return Transitions["OnBack"];

            return this;
        }
    }

}