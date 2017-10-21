using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TargetZone : MonoBehaviour {
	public GameObject LockOnHUDDisplay;

	private List<Target> trackedTargets = new List<Target>();

	private int raycastMask;

	// Use this for initialization
	void Start () {
		raycastMask = (1 << LayerMask.NameToLayer("TargetZone"));
	}
	
	// Update is called once per frame
	void Update () {
		if (Tracking)
		{
			foreach (Target target in trackedTargets)
			{
				RaycastHit hit;

				Physics.Raycast(new Ray(transform.position, (target.transform.position - transform.position).normalized), out hit, Mathf.Infinity, ~raycastMask);

				bool targetInSight = false;

				if(hit.transform == target.transform)
				{
					Vector3 direction = (GetComponent<Collider>().bounds.center - hit.point).normalized;

					if(!Physics.Raycast(hit.point, direction, Mathf.Infinity, raycastMask))
						targetInSight = true;
				}

				if (targetInSight)
				{
					if (!target.LockingOn && !target.LockedOn)
						target.BeginLockOn();
				}
				else
				{
					if (target.LockingOn || target.LockedOn)
						target.EndLockOn();
				}
			}
		}
		else
		{
			foreach (Target target in trackedTargets)
			{
				if (target.LockingOn || target.LockedOn)
					target.EndLockOn();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Target otherTarget = other.GetComponent<Target>();

		if(otherTarget != null && !trackedTargets.Contains(otherTarget))
			trackedTargets.Add(otherTarget);
	}

	private void OnTriggerExit(Collider other)
	{
		Target otherTarget = other.GetComponent<Target>();

		if (otherTarget != null && trackedTargets.Contains(otherTarget))
		{
			otherTarget.EndLockOn();
			trackedTargets.Remove(otherTarget);
		}
	}

	private bool tracking;
	public bool Tracking
	{
		get { return tracking; }
		set
		{
			tracking = value;
			LockOnHUDDisplay.SetActive(value);
		}
	}

	public List<Target> TrackedTargets
	{
		get { return trackedTargets; }
	}
}
