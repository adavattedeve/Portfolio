using UnityEngine;
using System.Collections;

public class MoveAction : Action {

    public int targetX;
    public int targetY;
    
    public MoveAction(int _x, int _y, Entity _actor) : base(_actor)
    {
        targetX = _x;
        targetY = _y;
    }

    public override void ProcesAction()
    {
        base.ProcesAction();
        //Tile tile = Grid.GetTile(targetX, targetY);
        Tile newTile = GameManager.Grid.GetTile(targetX, targetY);
        if (newTile.Moveable)
        {
            actor.CurrentTile.RemoveEntity(actor);
            newTile.AddEntity(actor);
            //move to the new tile
            (actor.go.GetComponent(typeof(IVisualizationController)) as IVisualizationController).Move(targetX, targetY, new System.Action(() => { actionFinished = true; }));
        }
        else
        {
            newTile.Collide(actor);
            actionFinished = true;
        }

    }
}
