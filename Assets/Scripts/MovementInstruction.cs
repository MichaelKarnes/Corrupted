using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MovementInstruction : MonoBehaviour {
	public Vector3 Destination;
	public bool UseWorldSpace;
	public float Speed;
	public UnityEvent OnEnd;

	private Vector3 destinationWorld;

	// Use this for initialization
	void Start () {
		destinationWorld = UseWorldSpace ? Destination : transform.TransformPoint(Destination);
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 destinationCurrent = destinationWorld - transform.position;
		Vector3 movementSpeed = destinationCurrent.normalized * Speed * Time.deltaTime;

		// If going at the specified Speed would overshoot the destination, we need to just set the
		// position equal to the destination
		if (movementSpeed.magnitude < destinationCurrent.magnitude)
			transform.position += movementSpeed;
		else
			transform.position = destinationWorld;

		if(transform.position == destinationWorld)
		{
			OnEnd.Invoke();
			enabled = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (enabled)
		{
			Gizmos.color = new Color(0f, 0.5f, 1f);
			Vector3 path;
			if (UseWorldSpace)
				path = Destination - transform.position;
			else
				path = transform.TransformPoint(Destination) - transform.position;
			Gizmos.DrawRay(transform.position, path);
		}
	}
}
