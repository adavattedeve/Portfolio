using UnityEngine;
using System.Collections;

public interface ICharacterInput {
	void RefreshInput();
	void IngoreInput(float t);
}
