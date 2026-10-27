using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    List<StateTransition> transitions { get; }
    void Enter();
    void Exit();
    void LogicUpdate();
    void OnSwitch();
}
