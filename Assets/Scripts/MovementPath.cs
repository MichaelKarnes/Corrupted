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

	private List<Vector3> NodesWorld = new List<Vector3>();
	private int currentNodeIndex = 0;

	// Use this for initialization
	void Start () {
		if (Nodes.Count > 0)
			NodesWorld.Add(transform.position);

		foreach(Vector3 node in Nodes)
			NodesWorld.Add(UseWorldSpace ? node : transform.TransformPoint(node));

		if (Nodes.Count > 0)
			OnEndPath();
		else
			Debug.Log("There must be at least one Node in the MovementPath");
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void OnEndPath()
	{
		currentNodeIndex++;

		if (Loop)
			currentNodeIndex %= NodesWorld.Count;

		if (currentNodeIndex < NodesWorld.Count)
		{
			UnityEvent instructionOnEnd = new UnityEvent();
			instructionOnEnd.AddListener(OnEndPath);

			MovementInstruction currentInstruction = gameObject.AddComponent<MovementInstruction>();
			currentInstruction.Destination = NodesWorld[currentNodeIndex];
			currentInstruction.UseWorldSpace = true;
			currentInstruction.Speed = Speed;
			currentInstruction.OnEnd = instructionOnEnd;
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
