using UnityEngine;
using System.Collections;

[System.Serializable]
public class MovementAnimation {
    [SerializeField]
    private Vector2 animatorParams;
    public Vector2 AnimatorParams { get { return animatorParams; } }

    [SerializeField]
    private Vector2 angleRange;
    public Vector2 AngleRange { get { return angleRange; } }

    [SerializeField]
    private float angleOffset;
    public float AngleOffset { get { return angleOffset; } }
}
