using UnityEngine;

public class Bitcrusher : MonoBehaviour 
{
	public bool UpdateOnStart = true;

	[Range(30,1)]
	public int bitdepth = 30;

	[Range(1,300)] 
	public int sampleRateReduction = 1;
	
	[Range(0,1.0f)] 
	public float volume = 0.7f;

	[Range(0,1)] 
	public float dryWet = 1f;

	private float leftholdValue = 0.0f;
	private float rightholdValue = 0.0f;
	private int holdCount = 0;

	void Start ()
	{
		if (UpdateOnStart == true) 
		{
			bitdepth = 				BitcrusherGroup.Instance.bitdepth;
			sampleRateReduction = 	BitcrusherGroup.Instance.sampleRateReduction;
			volume = 				BitcrusherGroup.Instance.volume;
			dryWet = 				BitcrusherGroup.Instance.dryWet;
		}
	}

	// Reads incoming samples and processes the bitcrush effect
    void OnAudioFilterRead (float[] data, int channels)
    {
        float bcMult = (1 << (bitdepth - 1));
        
		if (channels == 2)
        {
            for (int i = 0; i <  data.Length; i+=2)
            {
                if (holdCount == 0)
                {
                    holdCount = sampleRateReduction;
                    leftholdValue = ((int)(data [i] * bcMult)) / bcMult;
                    rightholdValue = ((int)(data [i + 1] * bcMult)) / bcMult;
                }

                data [i] = volume * (data [i] * (1 - dryWet) + (dryWet) * (leftholdValue));
                data [i + 1] = volume * (data [i + 1] * (1 - dryWet) + (dryWet) * (rightholdValue));
                holdCount--;
            }
        } 

		else
        
		{
            for (int i = 0; i < data.Length; i++)
            {
                if (holdCount == 0)
                {
                    holdCount = sampleRateReduction;
                    leftholdValue = ((int)(data [i] * bcMult)) / bcMult;
                }

                data [i] = volume * (data [i] * dryWet + (1 - dryWet) * (leftholdValue));
                holdCount--;
            }
        }
    }
}
