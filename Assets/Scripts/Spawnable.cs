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

	private Vector3 spawnPos;

	void Start()
	{
		spawnPos = transform.position;
	}

	public void Spawn()
	{
		spawnPos = transform.position;
		OnSpawn.Invoke();
	}

	public void Spawn(float seconds)
	{
		if (seconds > 0)
			StartCoroutine(WaitSpawn(seconds));
		else
			Spawn();
	}

	public void GoToSpawnPosition()
	{
		transform.position = spawnPos;
	}

	private IEnumerator WaitSpawn(float seconds)
	{
		yield return new WaitForSeconds(seconds);

		Spawn();
	}
}
