using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Slave
{
    public class SlaveWalkState : MonoBaseState
    {
        [SerializeField] Transform house;
        [HideInInspector]public Guard.Guard guard;
        CapsuleCollider capsule;

        List<Transform> path = new List<Transform>();
        Transform target;

        [SerializeField] float speed;

        bool isWaitingForPath;
        bool hasArrive;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isMining", false);
            animator.speed = 1;

            hasArrive = false;

            StartCoroutine(SetGuard());

            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();

        }
        IEnumerator SetGuard()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            while (guard == null)
            {
                var query = myCharacter.circleQuery.Query().Where(x => x.GetType() == typeof(Guard.Guard)).Cast<Guard.Guard>();

                if (query.Count() > 0)
                {
                    guard = query.OrderBy(x => (transform.position - x.transform.position).magnitude).First();
                    break;
                }
                yield return new WaitForSecondsRealtime(0.1f);
            }
        }

        public override void UpdateLoop()
        {
            if (guard == null)
                return;

            if (guard.IsDead)
            {
                target = house;
                return;
            }

            if ((transform.position - guard.transform.position).magnitude < 1f)
            {
                hasArrive = true;
                return;
            }

            if (transform.position.CanPassThrough(guard.transform.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = guard.transform;

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
            var nodes = myCharacter.GenerateStartNEnd(transform, guard.transform);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                path.Clear();
                path = x.Select(x => x.transform).PrependAppend(transform, guard.transform).ToList();//IA2-LINQ
                target = path.First();
                isWaitingForPath = false;
            }
            ));
        }
        public override IState ProcessInput()
        {
            if (hasArrive && Transitions.ContainsKey("OnGuard"))
            {
                hasArrive = false;
                return Transitions["OnGuard"];
            }


            return this;
        }
    }
}
