using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FSM.Guard
{
    public class GuardAttackState : MonoBaseState
    {
        Character target;

        [SerializeField] float speed;
        [SerializeField] int damage = 25;

        bool isAttacking;

        CapsuleCollider capsule;
        bool noenemies;

        public override void Enter(IState from, Dictionary<string, object> transitionParameters = null)
        {
            base.Enter(from, transitionParameters);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isEscorting", false);
            animator.speed = 1;

            if (capsule == null)
                capsule = GetComponent<CapsuleCollider>();

            noenemies = false;
        }
        public override void UpdateLoop()
        {//IA2-LINQ
            var entitys = myCharacter.circleQuery.Query()
                .Select(x => (Character)x)
                .Where(x => x.GetType() == typeof(Looter.Looter))
                .Where(x => x != null)
                .Where(x => transform.position
                        .CanPassThrough(x.transform.position, capsule.radius, capsule.height, GameManager.gm.wallLayer))
                .OrderBy(x=>(transform.position-x.transform.position).magnitude)
                .Take(1)
                .ToArray();

            if (entitys.Length >= 1)
                target = entitys[0];
            else
            {
                noenemies = true;
                return;
            }

            Vector3 dir = target.Position - myCharacter.Position;
            dir.y = 0;

            if (dir.magnitude < 0.5f)
            {
                if (!isAttacking)
                {
                    animator.SetBool("isAttacking", true);
                    isAttacking = true;
                }
                return;
            }

            if (isAttacking)
            {
                animator.SetBool("isAttacking", false);
                isAttacking = false;
            }

            transform.position += dir.normalized * speed * Time.deltaTime;
            transform.forward = dir;
        }
        public void TakeDamage()
        {
            if (target == null || myCharacter.Health <= 0)
                return;

            target.OnTakeDamage(damage);
        }
        public override IState ProcessInput()
        {
            if ((target != null && target.IsDead) || noenemies && Transitions.ContainsKey("OnIdle"))
                return Transitions["OnIdle"];

            return this;
        }
    }

}
