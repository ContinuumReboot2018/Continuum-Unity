using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Lens: MonoBehaviour 
{
	public Shader shader; // Must reference gravitational lensing shader.
	protected static Shader StaticShader;

	public float ratio = 1; // The ratio of the height to the length of the screen to display properly shader.

	public List<BlackHole> BlackHoles;

	[System.Serializable]
	public class BlackHole
	{
		public bool enabled;
		public bool useLocalPos;
		public float radius;
		public Transform BH;
		[HideInInspector]
		public Vector2 BHScreenPos;

		private Material _material; // Material which is located shader.
		public Material material
		{
			get 
			{
				if (_material == null) 
				{
					#if UNITY_EDITOR
					_material = new Material(Shader.Find ("Gravitation Lensing Shader"));
					#else
					_material = new Material (Lens.StaticShader);
					#endif
					_material.hideFlags = HideFlags.HideAndDontSave;
				}

				return _material;
			} 
		}

		public void DestroyMaterial ()
		{
			if (_material != null) 
			{
				DestroyImmediate (_material);
			}
		}

		public BlackHole (bool _enabled, bool _useLocalPos, Transform _BH, Vector2 _BHScreenPos, float _radius)
		{
			enabled = _enabled;
			useLocalPos = _useLocalPos;
			BH = _BH;
			BHScreenPos = _BHScreenPos;
			radius = _radius;
		}
	}

	void Awake ()
	{
		if (shader != null) 
		{
			StaticShader = shader;
		}
	}
		
	protected virtual void OnDisable () 
	{
		for (int i = 0; i < BlackHoles.Count; i++) 
		{
			BlackHoles [i].DestroyMaterial ();
		}
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (shader != null) 
		{
			RenderTexture TextureSource = new RenderTexture (source.width, source.height, 24);
			RenderTexture TextureDestination = source;

			for (int i = 0; i < BlackHoles.Count; i++) 
			{
				if (BlackHoles [i].enabled == true && BlackHoles[i].material != null) 
				{
					TextureSource = TextureDestination;
					TextureDestination = RenderTexture.GetTemporary (destination.width, destination.height);

					// Gets the current black hole offset in screen coordinates.
					Vector2 ScreenCoord = new Vector2 (
						BlackHoles [i].useLocalPos ? 
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.localPosition).x / 
						this.GetComponent<Camera> ().pixelWidth:
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.position).x / 
						this.GetComponent<Camera> ().pixelWidth,

						BlackHoles [i].useLocalPos ? 
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.localPosition).y / 
						this.GetComponent<Camera> ().pixelHeight:
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.position).y / 
						this.GetComponent<Camera> ().pixelHeight
					);
								
					// Find the position of the black hole in screen coordinates
					BlackHoles [i].BHScreenPos = ScreenCoord;

					// Install all parameters for each black hole.
					BlackHoles [i].material.SetVector ("_Position", ScreenCoord);
					BlackHoles [i].material.SetFloat ("_Ratio", ratio);
					BlackHoles [i].material.SetFloat ("_Rad", BlackHoles [i].radius);
					BlackHoles [i].material.SetFloat (
						"_Distance", 
						Vector3.Distance (
						BlackHoles [i].BH.position, this.transform.position
						)
					);

					Graphics.Blit (TextureSource, TextureDestination, BlackHoles [i].material);
				}
			}

			Graphics.Blit (TextureDestination, destination);
			TextureDestination.Release ();
		}
	}
}