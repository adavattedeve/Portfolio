using UnityEngine;
using System.Collections;

public class Shotgun : Gun {

    public ProjectileSplitter splitterComponent;

    protected override void Init() {
        weaponEffects.Add(splitterComponent);
    }
}
