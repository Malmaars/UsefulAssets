using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseState : State
{
    private System.Type previousState;

    private GameObject pauseVisual, visualPrefab;

    public PauseState() : base()
    {
        visualPrefab = Resources.Load("Interface/PausedText") as GameObject;
        pauseVisual = Object.Instantiate(visualPrefab);
        pauseVisual.SetActive(false);
    }

    //This should be called whenever you switch to the pause state, the previous state will be put through, and recalled when you unpause
    public void GetPreviousState(System.Type _previousState)
    {
        transitions = new List<StateTransition>();
        previousState = _previousState;
        Debug.Log(previousState);
        transitions.Add(new StateTransition(previousState, () => Input.GetKeyDown(KeyCode.Escape)));
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("pausing game");
        pauseVisual.SetActive(true);
        Time.timeScale = 0;

        //TODO: show visual of the game being paused
    }

    public override void Exit()
    {
        Debug.Log("unpausing game");
        base.Exit();
        pauseVisual.SetActive(false);
        Time.timeScale = 1;
    }
}
