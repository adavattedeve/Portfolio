using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {
    private CameraControl camControl;

	void Awake () {
        camControl = Camera.main.GetComponent<CameraControl>();
        camControl.Follow(gameObject);
    }

	void Update () {
        if (!GameManager.GameLogic.processingUpdate) {
            int h = (int)Input.GetAxisRaw("Horizontal");
            int v = (int)Input.GetAxisRaw("Vertical");
            if (h != 0) {
                GameManager.GameLogic.StartGameUpdate(new MoveAction(GameManager.player.CurrentTile.X + h, GameManager.player.CurrentTile.Y, GameManager.player));
            }
            else if (v != 0) {
                GameManager.GameLogic.StartGameUpdate(new MoveAction(GameManager.player.CurrentTile.X, GameManager.player.CurrentTile.Y + v, GameManager.player));
            }
        }
	}
}
