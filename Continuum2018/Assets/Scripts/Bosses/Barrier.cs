using UnityEngine;

public class Barrier : MonoBehaviour 
{
	public GameObject BarrierExplosion;

	void OnCollisionEnter (Collision col)
	{
		if (col.collider.tag == "Bullet") 
		{
			ExplodeBullet (col.transform, col.gameObject);
		}
	}

	void OnParticleCollision (GameObject other)
	{
		if (other.tag == "Bullet") 
		{
			ExplodeBullet (other.transform, other);
		}
	}

	void ExplodeBullet (Transform pos, GameObject bullet)
	{
		Instantiate (BarrierExplosion, pos.position, pos.rotation);
		Destroy (bullet);
		return;
	}
}