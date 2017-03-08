using UnityEngine;
using System.Collections;

public interface IVisualizationController
{
    void Move(int _x, int _y, System.Action callback);
}
