using UnityEngine;
using System.Collections;

public class SetUpExample1 : MonoBehaviour {

	public GameObject Bitcrusher;
	public Bitcrusher scene1Bitcrusher;

	void OnGUI ()    
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3(Screen.width / 800.0f, Screen.height / 700.0f, 1));

		scene1Bitcrusher.bitdepth = (int)(GUI.HorizontalSlider (new Rect (346, 157, 140, 30),(float)scene1Bitcrusher.bitdepth, 30, 1));
		scene1Bitcrusher.sampleRateReduction = (int)(GUI.HorizontalSlider (new Rect (346, 224, 140, 30),(float)scene1Bitcrusher.sampleRateReduction, 1, 300));
		scene1Bitcrusher.volume = GUI.HorizontalSlider (new Rect (346, 291, 140, 30), scene1Bitcrusher.volume, 0.0f, 1.0f);
		scene1Bitcrusher.dryWet = GUI.HorizontalSlider (new Rect (346, 357, 140, 30), scene1Bitcrusher.dryWet, 0.0f, 1.0f);
			
		GUI.Box (new Rect (506, 152, 35, 24), scene1Bitcrusher.bitdepth.ToString ("F0"));
		GUI.Box (new Rect (506, 221, 35, 24), scene1Bitcrusher.sampleRateReduction.ToString ("F0"));
		GUI.Box (new Rect (506, 289, 35, 24), scene1Bitcrusher.volume.ToString ("F2"));
		GUI.Box (new Rect (506, 358, 35, 24), scene1Bitcrusher.dryWet.ToString ("F2"));
	}
}