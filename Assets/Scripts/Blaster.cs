using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blaster : MonoBehaviour {
	public TargetZone TargetZone;
	public Transform Trigger;
	public Transform DischargePoint;
	public short MaxCharge = 4;
	public float AimAngle = 15f;

	private bool charging = false;
	private short charge = 0;

	private Coroutine coroutineCharge;
	private Coroutine coroutinePulse;

	private OVRInput.Vibration currentVibration = new OVRInput.Vibration(0f, 0f);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Trigger.localPosition = new Vector3(0f, OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) * -0.2f, 0f);
		if(!charging && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) > 0f)
		{
			ChargeBegin();
		}
		else if(charging && OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch) <= 0f)
		{
			ChargeEnd();
		}

		List<Target> trackedTargets = TargetZone.TrackedTargets;

		foreach (Target target in trackedTargets)
		{
			if (target.CurrentHealth > 0)
			{
				if (!target.AimedAt && Vector3.Angle(DischargePoint.up, (target.transform.position - DischargePoint.position).normalized) <= AimAngle)
					target.BeginAim();
				else if(target.AimedAt && Vector3.Angle(DischargePoint.up, (target.transform.position - DischargePoint.position).normalized) > AimAngle)
					target.EndAim();
			}
		}
	}

	private void ChargeBegin()
	{
		charging = true;

		coroutineCharge = StartCoroutine(Charge());

		TargetZone.Tracking = true;
	}

	private void ChargeEnd()
	{
		charging = false;

		if (coroutineCharge != null)
		{
			StopCoroutine(coroutineCharge);
			coroutineCharge = null;
		}

		if (coroutineCharge != null)
		{
			StopCoroutine(coroutinePulse);
			coroutinePulse = null;
		}

		if (charge > 0)
			StartCoroutine(Fire());
		else
			PowerDown();

		charge = 0;
		TargetZone.Tracking = false;
	}

	private IEnumerator Charge()
	{
		SetVibration(new OVRInput.Vibration(0.1f, 0.2f));

		yield return new WaitForSeconds(1f);

		charge++;

		if (charge < MaxCharge)
			coroutineCharge = StartCoroutine(Charge());
		else
			SetVibration(new OVRInput.Vibration(0f, 0f));

		coroutinePulse = StartCoroutine(Pulse());
	}

	private IEnumerator Pulse()
	{
		OVRInput.Vibration storedVibration = currentVibration;

		SetVibration(new OVRInput.Vibration(0.1f, 0.7f));

		yield return new WaitForSeconds(0.1f);

		SetVibration(storedVibration);
	}

	private IEnumerator Fire()
	{
		List<Target> trackedTargets = TargetZone.TrackedTargets;

		foreach(Target target in trackedTargets)
		{
			if (target.LockedOn && target.AimedAt)
				target.Damage(charge);
		}

		SetVibration(new OVRInput.Vibration(0.5f, 0.7f));

		yield return new WaitForSeconds(0.1f);

		SetVibration(new OVRInput.Vibration(0f, 0f));
	}

	private void PowerDown()
	{
		SetVibration(new OVRInput.Vibration(0f, 0f));
	}

	private void SetVibration(OVRInput.Vibration vibration)
	{
		currentVibration = vibration;
		OVRInput.SetControllerVibration(currentVibration, OVRInput.Controller.RTouch);
	}
}
