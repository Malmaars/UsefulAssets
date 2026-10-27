using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class State : IState
{
    public State()
    {
        transitions = new List<StateTransition>();
    }

    public List<StateTransition> transitions { get; protected set; }

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void LogicUpdate() { }

    public virtual void OnSwitch() { }
}