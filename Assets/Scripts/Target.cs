using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Target : MonoBehaviour {
	public Transform CenterEyeAnchor;
	public GameObject LockOnSprite;
	public GameObject AimSprite;
	public int Health;
	public int Armor;
	public float LockOnTime;

	public UnityEvent OnDie;

	private int currentHealth;

	private float lockOnStart = -1f;

	private GameObject lockOnSpriteInstance = null;
	private GameObject aimSpriteInstance = null;

	private Renderer targetRenderer;

	// Use this for initialization
	void Start () {
		currentHealth = Health;
		targetRenderer = GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
		if (lockOnSpriteInstance != null || aimSpriteInstance != null)
		{
			Vector3 targetCenterEyeAnchor = (transform.position - CenterEyeAnchor.position).normalized;

			Vector3 targetExtents = targetRenderer.bounds.extents;
			float maxRadius = Mathf.Max(targetExtents.x, targetExtents.y, targetExtents.z);

			if (lockOnSpriteInstance != null)
			{
				lockOnSpriteInstance.transform.rotation = Quaternion.LookRotation(targetCenterEyeAnchor, Vector3.ProjectOnPlane(Vector3.up, CenterEyeAnchor.forward));
				lockOnSpriteInstance.transform.position = transform.position - targetCenterEyeAnchor * maxRadius;
			}

			if (aimSpriteInstance != null)
			{
				aimSpriteInstance.transform.rotation = Quaternion.LookRotation(targetCenterEyeAnchor, Vector3.ProjectOnPlane(Vector3.up, CenterEyeAnchor.forward));
				aimSpriteInstance.transform.position = transform.position - targetCenterEyeAnchor * maxRadius;
			}
		}
	}

	public void BeginLockOn()
	{
		lockOnStart = Time.time;

		if (lockOnSpriteInstance == null)
		{
			lockOnSpriteInstance = Instantiate(LockOnSprite);

			Vector3 targetCenterEyeAnchor = (transform.position - CenterEyeAnchor.position).normalized;

			Vector3 extents = targetRenderer.bounds.extents;
			float maxRadius = Mathf.Max(extents.x, extents.y, extents.z);

			lockOnSpriteInstance.transform.rotation = Quaternion.LookRotation(targetCenterEyeAnchor, Vector3.ProjectOnPlane(Vector3.up, CenterEyeAnchor.forward));
			lockOnSpriteInstance.transform.position = transform.position - targetCenterEyeAnchor * maxRadius;

			lockOnSpriteInstance.SetActive(true);

			lockOnSpriteInstance.GetComponent<Animator>().SetFloat("LockOnTime", 1f / LockOnTime);
		}
	}

	public void EndLockOn()
	{
		lockOnStart = -1f;

		if (lockOnSpriteInstance != null)
		{
			Destroy(lockOnSpriteInstance);
			lockOnSpriteInstance = null;
		}
	}

	public void BeginAim()
	{
		if (aimSpriteInstance == null)
		{
			aimSpriteInstance = Instantiate(AimSprite);

			Vector3 targetCenterEyeAnchor = (transform.position - CenterEyeAnchor.position).normalized;

			Vector3 extents = targetRenderer.bounds.extents;
			float maxRadius = Mathf.Max(extents.x, extents.y, extents.z);

			aimSpriteInstance.transform.rotation = Quaternion.LookRotation(targetCenterEyeAnchor, Vector3.ProjectOnPlane(Vector3.up, CenterEyeAnchor.forward));
			aimSpriteInstance.transform.position = transform.position - targetCenterEyeAnchor * maxRadius;

			aimSpriteInstance.SetActive(true);
		}
	}

	public void EndAim()
	{
		if (aimSpriteInstance != null)
		{
			Destroy(aimSpriteInstance);
			aimSpriteInstance = null;
		}
	}

	public void Damage(int val)
	{
		currentHealth -= val - Armor;

		if (currentHealth <= 0)
			Kill();
	}

	/// <summary>
	/// Soft-kills the Target. Scripts will still run, but the Renderer and Collider are disabled.
	/// </summary>
	public void Kill()
	{
		OnDie.Invoke();

		if (lockOnSpriteInstance != null)
		{
			Destroy(lockOnSpriteInstance);
			lockOnSpriteInstance = null;
		}

		if (aimSpriteInstance != null)
		{
			Destroy(aimSpriteInstance);
			aimSpriteInstance = null;
		}

		GetComponent<Renderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
	}

	/// <summary>
	/// Hard-kills the Target's GameObject. Scripts will no longer run.
	/// </summary>
	public void Destroy()
	{
		if (lockOnSpriteInstance != null)
			Destroy(lockOnSpriteInstance);

		if (aimSpriteInstance != null)
			Destroy(aimSpriteInstance);

		Destroy(gameObject);
	}

	public bool LockingOn
	{
		get { return lockOnStart >= 0f && Time.time - lockOnStart < LockOnTime; }
	}

	public bool LockedOn
	{
		get { return lockOnStart >= 0f && Time.time - lockOnStart >= LockOnTime; }
	}

	public bool AimedAt
	{
		get { return aimSpriteInstance != null; }
	}

	public int CurrentHealth
	{
		get { return currentHealth; }
		set { currentHealth = value; }
	}
}
