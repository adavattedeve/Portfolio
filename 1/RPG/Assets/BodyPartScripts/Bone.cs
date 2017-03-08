using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bone  {
	public string name;
	private Transform transform;
	private Vector3 localPosition;
	private Quaternion localRotation;
	private Vector3 localScale;
	private List<Bone> children = new List<Bone>();

	public Bone(Transform transform)
	{
		this.transform = transform;
		name = transform.name;
		localPosition = transform.localPosition;
		localRotation = transform.localRotation;
		localScale = transform.localScale;
		foreach (Transform child in transform)
			children.Add(new Bone(child));
	}
	public void Restore()
	{
		transform.localPosition = localPosition;
		transform.localRotation = localRotation;
		transform.localScale = localScale;
		foreach (Bone child in children)
			child.Restore();
	}
}
