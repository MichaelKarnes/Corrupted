using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public partial class Spawnable : MonoBehaviour
{
	public UnityEvent OnSpawn;

	void Start()
	{
		
	}

	public void Spawn()
	{
		OnSpawn.Invoke();
	}

	public void Spawn(float seconds)
	{
		if (seconds > 0)
			StartCoroutine(WaitSpawn(seconds));
		else
			OnSpawn.Invoke();
	}

	private IEnumerator WaitSpawn(float seconds)
	{
		yield return new WaitForSeconds(seconds);

		OnSpawn.Invoke();
	}
}
