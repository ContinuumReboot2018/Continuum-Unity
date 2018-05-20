using UnityEngine;

[CreateAssetMenu(fileName = "New Ship", menuName = "Ship/Ship", order = 2)]
public class Ship : ScriptableObject
{
	public int ShipIndex;
	public Mesh ShipMesh;
	public Material ShipMaterial;
}