using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Lens: MonoBehaviour 
{
	public Shader shader; // Must reference gravitational lensing shader.
	protected static Shader StaticShader; // Create static shader instance and referenced by type.

	// The ratio of the height to the length of the screen to display shader.
	public float ratio = 1; // For 16:9 = 0.5625, For 16:10 = 0.625 (to achieve circular black holes).

	public List<BlackHole> BlackHoles; // All black holes accessible.

	[System.Serializable]
	public class BlackHole
	{
		public bool enabled; // Show black hole instance on screen.
		public bool useLocalPos; // Use local position to determine black hole position?
		public float radius; // How wide the black hole reaches.
		public Transform BH; // The current world location of the Black hole.
		[HideInInspector]
		public Vector2 BHScreenPos; // Where theblack hole is located in screen space.

		private Material _material; // Material which is located shader.

		// Creates new material instance if there is none.
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

		// Destroys the black hole material instance if there is one.
		public void DestroyMaterial ()
		{
			if (_material != null) 
			{
				DestroyImmediate (_material);
			}
		}

		// Set black hole parameters to instance settings.
		public BlackHole (bool _enabled, bool _useLocalPos, Transform _BH, Vector2 _BHScreenPos, float _radius)
		{
			enabled = _enabled;
			useLocalPos = _useLocalPos;
			BH = _BH;
			BHScreenPos = _BHScreenPos;
			radius = _radius;
		}
	}

	// Create static shader instance.
	void Awake ()
	{
		if (shader != null) 
		{
			StaticShader = shader;
		}
	}
		
	// Destroy the black hole material when script is disabled.
	protected virtual void OnDisable () 
	{
		for (int i = 0; i < BlackHoles.Count; i++) 
		{
			BlackHoles [i].DestroyMaterial ();
		}
	}

	// When the screen refreshes itself.
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (shader != null) 
		{
			RenderTexture TextureSource = new RenderTexture (source.width, source.height, 24);
			RenderTexture TextureDestination = source;

			for (int i = 0; i < BlackHoles.Count; i++) 
			{
				// For a black hole to be part of the Blit process, 
				// Black hole instance must be enabled, have a material assigned to it, and have a radius that is not 0.
				if (BlackHoles [i].enabled == true &&
				    BlackHoles [i].material != null &&
				    BlackHoles [i].radius != 0 &&
				    BlackHoles [i].BH.gameObject.activeInHierarchy == true) 
				{
					TextureSource = TextureDestination;
					TextureDestination = RenderTexture.GetTemporary (destination.width, destination.height);

					// Gets the current black hole offset in screen coordinates.
					Vector2 ScreenCoord = new Vector2 (
						BlackHoles [i].useLocalPos ? 
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.localPosition).x /
						this.GetComponent<Camera> ().pixelWidth :
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.position).x /
						this.GetComponent<Camera> ().pixelWidth,

						BlackHoles [i].useLocalPos ? 
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.localPosition).y /
						this.GetComponent<Camera> ().pixelHeight :
						this.GetComponent<Camera> ().WorldToScreenPoint (BlackHoles [i].BH.position).y /
						this.GetComponent<Camera> ().pixelHeight
					);
								
					// Find the position of the black hole in screen coordinates
					BlackHoles [i].BHScreenPos = ScreenCoord;

					// Install all parameters for each black hole.
					BlackHoles [i].material.SetVector ("_Position", ScreenCoord);
					BlackHoles [i].material.SetFloat ("_Ratio", ratio);
					BlackHoles [i].material.SetFloat ("_Rad", BlackHoles [i].radius);
					BlackHoles [i].material.SetFloat ("_Distance", 
						Vector3.Distance (BlackHoles [i].BH.position, this.transform.position));

					Graphics.Blit (TextureSource, TextureDestination, BlackHoles [i].material);
				} 

				else 
				
				{
					// Disable black hole if any conditions aren't met.
					BlackHoles [i].enabled = false;
				}
			}

			Graphics.Blit (TextureDestination, destination);
			TextureDestination.Release ();
		}
	}
}