using UnityEngine;
using System.Collections;

public class EnemyControl : CharacterControl {

	public override void SetDirection (Vector3 dir)
	{
		base.SetDirection (dir);
		if (lockOn) {
			direction = dir;
		}
		else {
			direction = dir;
			direction.y=0;
			
		}
	}

}
