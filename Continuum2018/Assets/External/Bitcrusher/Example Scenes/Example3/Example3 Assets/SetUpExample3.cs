using UnityEngine;
using System.Collections;

public class SetUpExample3 : MonoBehaviour {
	 
	public GameObject Cube;
	public Material CubePattern;
	public Material CubePatternA;
	public Material CubePatternB;
	public Material CubePatternC;
	public Material CubePatternX;
	public AudioClip OneShotExample;
	public Bitcrusher scene3Bitcrusher;

	void Update () {

	transform.Rotate (0, 2, 0);

	if (Input.GetKeyDown ("1")) 
		{
			GetComponent<AudioSource>().PlayOneShot(OneShotExample);
			Cube.GetComponent<Renderer>().material = CubePattern;
			scene3Bitcrusher.bitdepth = 30;
			scene3Bitcrusher.sampleRateReduction = 1;
		}

	if (Input.GetKeyDown ("2")) 
		{
			GetComponent<AudioSource>().PlayOneShot(OneShotExample);
			Cube.GetComponent<Renderer>().material = CubePatternA;
			scene3Bitcrusher.bitdepth = 8;
			scene3Bitcrusher.sampleRateReduction = 10;
		}

	if (Input.GetKeyDown ("3")) 
		{
			GetComponent<AudioSource>().PlayOneShot(OneShotExample);
			Cube.GetComponent<Renderer>().material = CubePatternB;
			scene3Bitcrusher.bitdepth = 8;
			scene3Bitcrusher.sampleRateReduction = 20;
		}

	if (Input.GetKeyDown ("4")) 
		{
			GetComponent<AudioSource>().PlayOneShot(OneShotExample);
			Cube.GetComponent<Renderer>().material = CubePatternC;
			scene3Bitcrusher.bitdepth = 8;
			scene3Bitcrusher.sampleRateReduction = 40;
		}

	if (Input.GetKeyDown ("5")) 
		{
			GetComponent<AudioSource>().PlayOneShot(OneShotExample);
			Cube.GetComponent<Renderer>().material = CubePatternX;
			scene3Bitcrusher.bitdepth = 8;
			scene3Bitcrusher.sampleRateReduction = 80;
		}
	}

	void OnGUI ()	
	{
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,new Vector3(Screen.width / 800.0f, Screen.height / 700.0f, 1));

		scene3Bitcrusher.sampleRateReduction = (int)(GUI.VerticalSlider (new Rect (190, 208, 40, 150),(float)scene3Bitcrusher.sampleRateReduction, 1, 300));
		GUI.Box (new Rect (178, 418, 35, 24), scene3Bitcrusher.sampleRateReduction.ToString ("F0"));
	}	
}