using UnityEngine;

[System.Serializable]
public class BlackHole
{
	public Transform BH;
	public float radius;
	[HideInInspector]
	public Material material;

	public BlackHole (Transform _BH, float _radius, Material _material)
	{
		BH = _BH;
		radius = _radius;
		material = _material;
	}
}