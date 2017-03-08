using UnityEngine;
using System.Collections;

public class Action  {
    public Entity actor;
    public bool actionFinished;
    public Action(Entity _actor)
    {
        actor = _actor;
        actionFinished = false;
    }

    public virtual void ProcesAction()
    {
    }
}
