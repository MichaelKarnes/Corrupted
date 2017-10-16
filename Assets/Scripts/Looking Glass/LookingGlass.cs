using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A LookingGlass is simply a plane that does something when the player is looking at it
/// </summary>
[RequireComponent(typeof(BoxCollider))]
public abstract class LookingGlass : MonoBehaviour {
	public Transform CenterEyeAnchor;

	[SerializeField]
	protected bool physicsBased = true;

	// Use this for initialization
	protected virtual void Start () {
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if(!physicsBased)
		{
			// Find point intersection between plane and looking ray
			float hitDistance = Vector3.Dot(transform.position - CenterEyeAnchor.position, transform.forward) / Vector3.Dot(transform.forward, CenterEyeAnchor.forward);

			if (hitDistance > 0)
			{
				Vector3 hitPoint = CenterEyeAnchor.position + CenterEyeAnchor.forward * hitDistance;

				if (GetComponent<BoxCollider>().bounds.Contains(hitPoint))
				{
					Look(hitPoint);
				}
			}
		}
	}

	public abstract void Look(Vector3 lookPoint);
}
