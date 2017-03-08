using UnityEngine;
using System.Collections;

public class GameLogic : MonoBehaviour {

    public bool processingUpdate = false;

    public void StartGameUpdate(Action playerAction)
    {
        StartCoroutine(GameUpdate(playerAction));
    }

    private IEnumerator GameUpdate(Action playerAction)
    {
        processingUpdate = true;
        playerAction.ProcesAction();

        while (!playerAction.actionFinished)
        {
            yield return null;
        }
        GameManager.instance.Fow.UpdateFow();
        processingUpdate = false;
    }
}
