using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Guard
{
    public class GuardEscortState : MonoBaseState
    {
        Transform slave;
        CapsuleCollider capsule;

        List<Transform> path = new List<Transform>();
        Transform target;

        [SerializeField] float speed;

        bool isWaitingForPath;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isEscorting", true);


            slave = myCharacter.circleQuery.Query().Where(x => x.GetType() == typeof(Slave.Slave)).Cast<Slave.Slave>().First().transform;
            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();
        }

        public override void UpdateLoop()
        {
            if ((transform.position - slave.position).magnitude < 1f)
            {
                animator.speed = 0;
                return;
            }
            else
            {
                animator.speed = 1;
            }

            if (transform.position.CanPassThrough(slave.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = slave;

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

                    if (dir.magnitude < 0.5f && path.Count > 0)
                    {//IA2-LINQ
                        path = path.Skip(1).ToList();//next Target
                    }

                    transform.position += dir.normalized * speed * Time.deltaTime;
                    transform.forward = dir;
                }
            }

        public void GeneratePath()
        {//IA2-LINQ

            isWaitingForPath = true;
            var nodes = myCharacter.GenerateStartNEnd(transform, slave);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                path.Clear();
                path = x.Select(x => x.transform).PrependAppend(transform, slave).ToList();//IA2-LINQ
                    target = path.First();
                isWaitingForPath = false;
            }
            ));
        }
        public override IState ProcessInput()
        {
            if(slave == null && Transitions.ContainsKey("OnIdle"))
                return Transitions["OnIdle"];
            else if (myCharacter.circleQuery.Query().Select(x => (Character)x).Where(x => x.GetType() == typeof(Looter.Looter)).Count() >= 1
                && Transitions.ContainsKey("OnAttack"))
                return Transitions["OnAttack"];


            return this;
        }
    }

}
