using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//since black and white both need a turn, but both turns are essentially identical, they can derive from a base turn class
//this also opens the possibility for more players 
public class Turn : Gamestate
{
    protected PlaceableColor turnsPlaceableColor;
    protected IPlaceable selectedPlaceable;
    protected bool turnFinished = false;
    protected bool victoryBool = false;
    protected bool combo;

    protected int currency;

    public Turn() : base()
    {
        //Transition to pause state when you press the escape button
        transitions.Add(new StateTransition(typeof(PauseState), () => Input.GetKeyDown(KeyCode.Escape)));

        //transition to endstate when you win
        transitions.Add(new StateTransition(typeof(EndState), () => victoryBool == true));
    }
    public override void Enter()
    {
        base.Enter();
        victoryBool = false;
        turnFinished = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (PotionManager.currentlySelectedPotion == null || (PotionManager.CheckWallet(turnsPlaceableColor) < 3))
            MovePieces();

        //we check if the player can even pay for the potion. If they can't, no need to use it
        else
            UsePotion();

        if (PotionManager.CheckWallet(turnsPlaceableColor) < 3)
            PotionManager.DeselectPotion();

    }

    private void MovePieces()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPos clickedPos = GridSystem.ClickOnTiles();
            Checker clickedTile = GridSystem.CheckGridPosition(clickedPos);

            if (selectedPlaceable != null)
            {
                GridSystem.ClearOptions();
                if (combo)
                {
                    if (clickedTile != null)
                    {
                        //keep the combo going if possible
                        if (GridSystem.AttackChecker(selectedPlaceable.myPos, clickedPos))
                        {
                            combo = GridSystem.CheckForPossibleAttacks(selectedPlaceable.myPos);
                            //if there are no more combo attacks, end turn
                            turnFinished = !combo;
                        }
                    }

                    //keep showing the attack options even if the player clicks on something else. The only option is to continue the combo
                    GridSystem.ShowAllAttackOptionsForPlaceable(selectedPlaceable.myPos);
                }

                else if(!combo)
                {
                    //if you click on a tile that's not empty, attack if possible
                    if (clickedTile != null)
                    {
                        //the attack is a little more complicated than the move. Since attacks are to be chained
                        if (GridSystem.AttackChecker(selectedPlaceable.myPos, clickedPos))
                        {
                            GridSystem.ShowAllAttackOptionsForPlaceable(selectedPlaceable.myPos);
                            //we've succesfully attacked something, let's scan the area for a possible combo
                            combo = GridSystem.CheckForPossibleAttacks(selectedPlaceable.myPos);
                            turnFinished = !combo;
                        }

                        //if you click on another piece of your color, switch to that one
                        if (clickedTile != null && clickedTile.color == turnsPlaceableColor)
                        {
                            selectedPlaceable = clickedTile;
                            GridSystem.ShowAllAttackOptionsForPlaceable(clickedPos);
                            GridSystem.ShowAllMovementOptionsForPlaceable(clickedPos);
                        }
                    }
                    //if you click on an empty tile, move if possible
                    else if (clickedTile == null)
                    {
                        turnFinished = GridSystem.MoveChecker(selectedPlaceable.myPos, clickedPos);
                        selectedPlaceable = null;
                    }
                }
            }

            //if there's not already a placeable selected, assign a new one
            else if (selectedPlaceable == null)
            {
                if (clickedTile != null && clickedTile.color == turnsPlaceableColor)
                {
                    selectedPlaceable = clickedTile;
                    GridSystem.ShowAllAttackOptionsForPlaceable(clickedPos);
                    GridSystem.ShowAllMovementOptionsForPlaceable(clickedPos);
                }
            }
        }

        else if (Input.GetMouseButton(1))
        {
            //nestled if statement, because it's possible we want more functionality inside this input

            //cancel selection
            if (selectedPlaceable != null)
            {
                selectedPlaceable = null;
                GridSystem.ClearOptions();
            }
        }

        if (turnFinished)
        {
            //check for victory
            if (GridSystem.CheckForVictory(turnsPlaceableColor))
            {
                //the color won. Don't end the turn, but go to the end screen instead
                Victory();
                turnFinished = false;
            }

            selectedPlaceable = null;
        }
    }

    private void Victory()
    {
        //a player wins when the only pieces left on the board are of the player's color
        GridSystem.SetWinner(turnsPlaceableColor);
        victoryBool = true;
    }

    private void UsePotion() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPos clickedPos = GridSystem.ClickOnTiles();

            if (!GridSystem.CheckIfGridposIsInsideGrid(clickedPos) || Input.GetMouseButtonDown(1))
            {
                //clickedPos is outside the grid
                PotionManager.DeselectPotion();
                return;
            }

            else
            {
                //clickedPos is inside grid. perform check if the cell can be used, and use the potion effect if so.
                PotionManager.UseEffect(clickedPos, turnsPlaceableColor);
            }
        }
    }
}
