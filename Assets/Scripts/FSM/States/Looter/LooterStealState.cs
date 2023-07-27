using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Looter
{
    public class LooterStealState : MonoBaseState
    {
        [SerializeField] Transform medievalHouse;
        List<Transform> path = new List<Transform>();

        [SerializeField] float speed;
        [SerializeField] int maxCapacity = 100;

        float speedbackup;
        int loot;
        bool isStealing;
        bool isWaitingForPath;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
            speedbackup = speed;
            isStealing = false;
            loot = 0;
            foreach (var item in Transitions)
            {
                Debug.Log(item.Key + ": " + item.Value);
            }
        }

        public override void UpdateLoop()
        {
            Vector3 target;
            if (transform.position.InLineOfSight(medievalHouse.position, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target
                target = medievalHouse.position;

                if (path.Count > 0)
                    path = new List<Transform>();
            }
            else if(path.Count<=0&& !isWaitingForPath)
            {//IA2-LINQ
                Node start = Physics.OverlapSphere(transform.position, 1f).Where(x => x.GetComponent<Node>() != null)
                    .Select(x => x.GetComponent<Node>()).OrderBy(x => (transform.position - x.transform.position).magnitude)
                    .First();

                Node end = Physics.OverlapSphere(medievalHouse.position, 1f).Where(x => x.GetComponent<Node>() != null)
                    .Select(x => x.GetComponent<Node>()).OrderBy(x => (medievalHouse.position - x.transform.position).magnitude)
                    .First();

                path.Add(start.transform);
                SetAnimVelocity(0);

                StartCoroutine(AStar.CalculatePath(start, (x) => x == end,
                (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
                x => x.heuristic,
                x =>{
                        path= x.Select(x=>x.transform).Prepend(start.transform).Append(end.transform).ToList();
                        SetAnimVelocity(speedbackup);//return velocity
                    }
                ));
            }


            Vector3 dir = medievalHouse.position - transform.position;
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
            if (loot == maxCapacity)
                animator.SetBool("isStealing", false);
        }
        public void SetAnimVelocity(float _speed)
        {
            speed = _speed;
            animator.speed = Mathf.Clamp01(_speed);
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

