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
        Transform target;

        [SerializeField] float speed;
        [SerializeField] int maxCapacity = 100;

        CapsuleCollider capsule;
        int loot;
        bool isStealing;
        bool isWaitingForPath;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isStealing", false);
            animator.SetBool("isAttacking", false);
            animator.speed = 1;

            isStealing = false;
            loot = 0;

            if(capsule==null)
                capsule=GetComponent<CapsuleCollider>();
        }

        public override void UpdateLoop()
        {
            if (transform.position.CanPassThrough(medievalHouse.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
            {//if is in line of sight, clean the list and add house as target

                target = medievalHouse;

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

                if ((transform.position - medievalHouse.position).magnitude < 0.5f)
                {
                    if (!isStealing)
                    {
                        animator.SetBool("isStealing", true);
                        isStealing = true;
                    }
                }
                else
                {
                    if (dir.magnitude < 0.5f && path.Count > 0)
                    {//IA2-LINQ
                        path = path.Skip(1).ToList();//next Target
                    }

                    if (isStealing)
                    {
                        animator.SetBool("isStealing", false);
                        isStealing = false;
                    }

                    transform.position += dir.normalized * speed * Time.deltaTime;
                    transform.forward = dir;
                }
            }
        }

        public void GeneratePath()
        {//IA2-LINQ

            isWaitingForPath = true;
            var nodes = myCharacter.GenerateStartNEnd(transform,medievalHouse);

            //A*
            StartCoroutine(AStar.CalculatePath(nodes.First(), (x) => x == nodes.Last(),
            (x) => x.neighbours.Select(x => new WeightedNode<Node>(x, 1)),
            x => x.heuristic,
            x => {
                    path.Clear();
                    path=x.Select(x => x.transform).PrependAppend(transform,medievalHouse).ToList();//IA2-LINQ
                    target = path.First();
                    isWaitingForPath = false;
                }
            ));
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

