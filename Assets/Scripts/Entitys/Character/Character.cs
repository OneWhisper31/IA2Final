using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FSM;
using System;
using System.Linq;

public abstract class Character : Entity
{
    FiniteStateMachine fsm;

    public abstract List<Tuple<string,IState,IState>> SetTransitions();//autocompletedInChilds

    public void Awake()
    {
        var transitions = SetTransitions();

        if (transitions.Count > 0)
            fsm = ConfigureFSM(transitions.First().Item2, StartCoroutine, transitions);//IA2-LINQ
    }

    public static FiniteStateMachine
        ConfigureFSM(IState initialState, Func<IEnumerator, Coroutine> startCoroutine, List<Tuple<string, IState, IState>> transitions)
        => transitions.Aggregate(new FiniteStateMachine(initialState, startCoroutine),(x,y)=> {//IA2-LINQ

            x.AddTransition("On" + y.Item1, y.Item2, y.Item3);

            return x;
    });
}
