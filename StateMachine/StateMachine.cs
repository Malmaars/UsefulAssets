using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateMachine
{
    private IState currenstate;

    //since there is only one instance of this statemachine, it won't need an objectPool
    //Rather I will use a dictionary to save each state version. Since there will only ever be one version of each state
    private Dictionary<System.Type, IState> stateCollection;

    public StateMachine(System.Type _initialState)
    {
        stateCollection = new Dictionary<System.Type, IState>();
        currenstate = GetState(_initialState);
    }

    public void SwitchState(IState _newState)
    {
        currenstate.OnSwitch();
        currenstate.Exit();
        currenstate = _newState;
        currenstate.Enter();
    }

    public void StateUpdate()
    {
        currenstate.LogicUpdate();


        //Check each transition of a state, and if it wants to switch, switch.
        foreach (StateTransition transition in currenstate.transitions)
        {
            if (transition.condition())
            {
                Debug.Log(transition.target);
                if (transition.target == typeof(PauseState))
                {
                    PauseState pause = GetState(transition.target) as PauseState;
                    pause.GetPreviousState(currenstate.GetType());
                }

                SwitchState(GetState(transition.target));
                break;
            }
        }
    }

    public IState GetState(System.Type _t)
    {
        return GetOrCreateState(_t);
    }

    private IState GetOrCreateState(System.Type _t)
    {
        IState state;

        //if statecollection has that type, fetch it
        if (stateCollection.TryGetValue(_t, out state))
        {
            return state;
        }

        //if it doesn't, make a new one.
        else
        {
            object obj = System.Activator.CreateInstance(_t);
            IState _instance = (IState)obj;

            stateCollection.Add(_t, _instance);

            return _instance;
        }
    }
}