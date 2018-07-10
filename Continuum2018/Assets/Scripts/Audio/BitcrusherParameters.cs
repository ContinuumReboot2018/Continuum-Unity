using UnityEngine;

[CreateAssetMenu(fileName = "New Bitcrusher Parameters", menuName = "Bitcrusher/Bitcrusher Parameters", order = 4)]
public class BitcrusherParameters : ScriptableObject
{
	[Header ("Bitcrusher Parameters")]
	[Range(30,1)]
	public int bitdepth = 30;

	[Range(1,300)] 
	public int sampleRateReduction = 1;

	[Range(0,1.0f)] 
	public float volume = 0.7f;

	[Range(0,1)] 
	public float dryWet = 1f;
}