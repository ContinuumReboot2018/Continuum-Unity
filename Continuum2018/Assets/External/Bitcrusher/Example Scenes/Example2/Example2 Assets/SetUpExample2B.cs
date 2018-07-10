using UnityEngine;
using System.Collections;

public class SetUpExample2B : MonoBehaviour {

	public GameObject BitcrusherB;
	public Bitcrusher scene2BBitcrusher;

	void OnGUI ()	
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3(Screen.width / 800.0f, Screen.height / 700.0f, 1));

		scene2BBitcrusher.sampleRateReduction = (int)(GUI.VerticalSlider (new Rect (606, 208, 40, 150),(float)scene2BBitcrusher.sampleRateReduction, 1, 50));
		GUI.Box (new Rect (593, 418, 35, 24), scene2BBitcrusher.sampleRateReduction.ToString ("F0"));
	}	
}