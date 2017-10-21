using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class MovementPath : MonoBehaviour {
	public List<Vector3> Nodes;
	public bool UseWorldSpace;
	public bool Loop;
	public float Speed;

	private List<Vector3> nodesWorld;
	private List<MovementInstruction> movementInstructions;
	private int currentNodeIndex;
	
	// Update is called once per frame
	void Update () {
	}

	void Start()
	{
		Reset();
	}

	public void Reset()
	{
		nodesWorld = new List<Vector3>();
		movementInstructions = new List<MovementInstruction>();
		currentNodeIndex = 0;

		if (Nodes.Count > 0)
			nodesWorld.Add(transform.position);

		foreach (Vector3 node in Nodes)
			nodesWorld.Add(UseWorldSpace ? node : transform.TransformPoint(node));

		if (Nodes.Count > 0)
		{
			foreach (Vector3 node in nodesWorld)
			{
				UnityEvent instructionOnEnd = new UnityEvent();
				instructionOnEnd.AddListener(OnEndPath);

				MovementInstruction currentInstruction = gameObject.AddComponent<MovementInstruction>();
				currentInstruction.Destination = node;
				currentInstruction.UseWorldSpace = true;
				currentInstruction.Speed = Speed;
				currentInstruction.OnEnd = instructionOnEnd;
				currentInstruction.enabled = false;

				movementInstructions.Add(currentInstruction);
			}

			OnEndPath();
		}
		else
			Debug.Log("There must be at least one Node in the MovementPath");
	}

	private void OnEnable()
	{
		if(movementInstructions != null && movementInstructions.Count > currentNodeIndex)
			movementInstructions[currentNodeIndex].enabled = true;
	}

	private void OnDisable()
	{
		foreach (MovementInstruction instruction in movementInstructions)
			instruction.enabled = false;
	}

	private void OnDestroy()
	{
		foreach (MovementInstruction instruction in movementInstructions)
			Destroy(instruction);
	}

	private void OnEndPath()
	{
		currentNodeIndex++;

		if (Loop)
			currentNodeIndex %= nodesWorld.Count;

		if (currentNodeIndex < nodesWorld.Count)
		{
			movementInstructions[currentNodeIndex].enabled = true;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (enabled)
		{
			Gizmos.color = new Color(0f, 0.5f, 1f);

			for (int i = 0; i < Nodes.Count; i++)
			{
				Vector3 initPos;
				Vector3 finalPos;

				if (UseWorldSpace)
				{
					initPos = i == 0 ? transform.position : Nodes[i - 1];
					finalPos = Nodes[i];
				}
				else
				{
					initPos = i == 0 ? transform.position : transform.TransformPoint(Nodes[i - 1]);
					finalPos = transform.TransformPoint(Nodes[i]);
				}

				Gizmos.DrawRay(initPos, finalPos - initPos);
			}

			if(Loop && Nodes.Count > 0)
			{
				Vector3 initPos;
				Vector3 finalPos;

				if (UseWorldSpace)
				{
					initPos = Nodes[Nodes.Count - 1];
					finalPos = transform.position;
				}
				else
				{
					initPos = transform.TransformPoint(Nodes[Nodes.Count - 1]);
					finalPos = transform.position;
				}

				Gizmos.DrawRay(initPos, finalPos - initPos);
			}
		}
	}
}
