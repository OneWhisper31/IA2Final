using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public abstract class Character : Entity
{
    FiniteStateMachine fsm;


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

    public static FiniteStateMachine
        ConfigureFSM(IState initialState, Func<IEnumerator, Coroutine> startCoroutine, TransitionMono[] transitions)
        => transitions.Aggregate(new FiniteStateMachine(initialState, startCoroutine),(x,y)=> {//IA2-LINQ

            x.AddTransition("On" + y.name, y.from, y.to);

            return x;
    });
}
[Serializable]
public struct TransitionMono
{
    public string name { get => from.name; }

    public MonoBaseState from;
    public MonoBaseState to;
}
