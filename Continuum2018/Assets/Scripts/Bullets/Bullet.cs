using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour 
{
	public float BulletSpeed;
	public Rigidbody BulletRb;

	public Transform DestroyPos;
	public float Lifetime;
	public float MaxLifetime = 30;

	public float ColliderYMaxPos = 12;
	public Collider BulletCol;

	void Start ()
	{
		BulletRb.velocity = transform.InverseTransformDirection (new Vector3 (0, BulletSpeed * Time.deltaTime, 0));
		Lifetime = 0;
		InvokeRepeating ("CheckForDestroy", 0, 1);
	}

	void Update ()
	{
		Lifetime += Time.unscaledDeltaTime;
		CheckForDestroy ();

		CheckForColliderDeactivate ();
	}

	void CheckForColliderDeactivate ()
	{
		if (BulletRb.transform.position.y > ColliderYMaxPos) 
		{
			BulletCol.enabled = false;
		}
	}

	void CheckForDestroy ()
	{
		if (Lifetime > MaxLifetime) 
		{
			Destroy (gameObject);
		}

		if (BulletRb.transform.position.y > DestroyPos.position.y) 
		{
			Destroy (gameObject);
		}
	}
}
