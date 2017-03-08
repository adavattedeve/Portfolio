using UnityEngine;
using System.Collections;

public interface IUpdateable {
    //returns false if modifier implementing this is no longer in effect.
    bool Update(float deltaTime);
}
