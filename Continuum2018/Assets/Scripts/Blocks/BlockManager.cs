using UnityEngine;

[CreateAssetMenu(fileName = "New Block Type", menuName = "Block Type")]
public class BlockManager : ScriptableObject 
{
	public float Speed;
	public Material Material;
	public int BasePointValue;
	public GameObject Explosion;
	public Color TextColor;
}