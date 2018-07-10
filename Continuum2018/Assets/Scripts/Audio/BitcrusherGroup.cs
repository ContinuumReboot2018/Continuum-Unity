using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitcrusherGroup : MonoBehaviour 
{
	public static BitcrusherGroup Instance { get; private set; }

	public Bitcrusher[] Bitcrushers;
	[Space (10)]
	public BitcrusherParameters CurrentBitcrusherparam;

	[Space (10)]
	public BitcrusherParameters NormalBitcrushParameters;
	public BitcrusherParameters OverrideBitcrushParameters;
	[Space (10)]

	public float UpdateRefreshParameterTime = 1;

	[Header ("Bitcrusher Parameters")]
	[Range(30,1)]
	public int bitdepth = 30;

	[Range(1,300)] 
	public int sampleRateReduction = 1;

	[Range(0,1.0f)] 
	public float volume = 0.7f;

	[Range(0,1)] 
	public float dryWet = 1f;

	void Awake ()
	{
		Instance = this;
	}

	IEnumerator BitcrushParameterRefresh ()
	{
		while (true) 
		{
			yield return new WaitForSecondsRealtime (UpdateRefreshParameterTime);
			UpdateBitcrusherParameters (CurrentBitcrusherparam);
			UpdateBitcrushers ();
		}
	}

	public void UpdateBitcrusherParameters (BitcrusherParameters bitcrushparam)
	{
		bitdepth = bitcrushparam.bitdepth;
		sampleRateReduction = bitcrushparam.sampleRateReduction;
		volume = bitcrushparam.volume;
		dryWet = bitcrushparam.dryWet;
	}

	public void UpdateBitcrushers ()
	{
		for (int i = 0; i < Bitcrushers.Length; i++) 
		{
			Bitcrushers [i].bitdepth = bitdepth;
			Bitcrushers [i].sampleRateReduction = sampleRateReduction;
			Bitcrushers [i].volume = volume;
			Bitcrushers [i].dryWet = dryWet;
		}
	}
}