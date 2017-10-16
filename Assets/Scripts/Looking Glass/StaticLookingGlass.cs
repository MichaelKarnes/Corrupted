using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A StaticLookingGlass is a LookingGlass that does not perform any transformations
/// </summary>
public class StaticLookingGlass : LookingGlass {
	[SerializeField]
	public bool PhysicsBased { get { return physicsBased; } set { physicsBased = value; } }

	public UnityEvent OnLook;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}

	public override void Look(Vector3 lookPoint)
	{
		OnLook.Invoke();
	}
}
