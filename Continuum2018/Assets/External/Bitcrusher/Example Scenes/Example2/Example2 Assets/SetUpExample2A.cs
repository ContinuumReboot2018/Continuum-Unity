using UnityEngine;
using System.Collections;

public class SetUpExample2A : MonoBehaviour {

	public GameObject BitcrusherA;
	public Bitcrusher scene2ABitcrusher;

	void OnGUI ()	
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3(Screen.width / 800.0f, Screen.height / 700.0f, 1)); 

		scene2ABitcrusher.sampleRateReduction = (int)(GUI.VerticalSlider (new Rect (190, 208, 40, 150),(float)scene2ABitcrusher.sampleRateReduction, 1, 300));
		GUI.Box (new Rect (178, 418, 35, 24), scene2ABitcrusher.sampleRateReduction.ToString ("F0"));
	}	    
}