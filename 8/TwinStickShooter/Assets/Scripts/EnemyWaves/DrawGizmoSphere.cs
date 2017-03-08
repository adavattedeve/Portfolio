using UnityEngine;
using System.Collections;

public class DrawGizmoSphere : MonoBehaviour {
    public Color color = Color.white;
    public float radius = 1;
    public Vector3 offset = Vector3.zero;

    public void SetValues (Color _color, float _radius, Vector3 _offset)
    {
        color = _color;
        radius = _radius;
        offset = _offset;
    }
    public void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position + offset, radius);

    }
}
