using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public abstract class Character : Entity
{
    FiniteStateMachine fsm;
    [SerializeField] Transform respawnPoint;

    [SerializeField] List<TransitionMono> transitions = new List<TransitionMono>();

    public virtual TransitionMono[] SetTransitions()
    {
        return transitions.ToArray();//IA2-LINQ
    }

    public void Awake()
    {
        var transitions = SetTransitions();

        if (transitions.Length > 0)
            fsm = ConfigureFSM(transitions.First().from, StartCoroutine, transitions);//IA2-LINQ
        fsm.Active = true;
    }
    public void Update()
    {
        if (IsDead)
        {
            fsm.Active = false;

        }
    }
    public override void OnDeadEvent()
    {
        fsm.Active = false;
        transform.position = GameManager.gm.deadPivot.position;
        StartCoroutine(Respawn());

    }
    public IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(5,16));
        transform.position = respawnPoint.position;

        var transitions = SetTransitions();

        if (transitions.Length > 0)
            fsm = ConfigureFSM(transitions.First().from, StartCoroutine, transitions);//IA2-LINQ
        fsm.Active = true;

        OnFullHealEvent();
    }

    public static FiniteStateMachine
        ConfigureFSM(IState initialState, Func<IEnumerator, Coroutine> startCoroutine, TransitionMono[] transitions)
        => transitions.Aggregate(new FiniteStateMachine(initialState, startCoroutine),(x,y)=> {//IA2-LINQ

            x.AddTransition(y.Name, y.from, y.to);

            return x;
    });

    //A*
    public List<Node> GenerateStartNEnd(Transform startPos, Transform endPos)
    {//IA2-LINQ
        List<Node> nodes = new List<Node>();

        nodes.Add(Physics.OverlapSphere(startPos.position, 1f).Where(x => x.GetComponent<Node>() != null)
            .Select(x => x.GetComponent<Node>()).OrderBy(x => (startPos.position - x.transform.position).magnitude)
            .First());

        nodes.Add(Physics.OverlapSphere(endPos.position, 1f).Where(x => x.GetComponent<Node>() != null)
            .Select(x => x.GetComponent<Node>()).OrderBy(x => (endPos.position - x.transform.position).magnitude)
            .First());

        return nodes;
    }
}
[Serializable]
public struct TransitionMono
{
    [SerializeField] string name;
    public string Name { get => "On" + name; }

    public MonoBaseState from;
    public MonoBaseState to;
}
