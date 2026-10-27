using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate bool Condition();

public class StateTransition
{
    public System.Type target;
    public Condition condition;

    public StateTransition(System.Type _target, Condition _condition)
    {
        target = _target;
        condition = _condition;
    }
}
