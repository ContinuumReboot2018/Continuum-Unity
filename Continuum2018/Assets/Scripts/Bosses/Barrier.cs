using UnityEngine;

public class Barrier : MonoBehaviour 
{
	[Tooltip ("Explosion to play when a barrier collides with a bullet.")]
	public GameObject BarrierExplosion;

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