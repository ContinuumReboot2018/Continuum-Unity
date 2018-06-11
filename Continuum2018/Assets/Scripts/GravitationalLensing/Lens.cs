using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[ExecuteInEditMode]
public class Lens: MonoBehaviour 
{
	public bool UseLocalPos; // Use local position?
	public Shader Shader; // Must reference gravitational lensing shader.
    protected static Shader StaticShader;
	
	public float Ratio = 1; // The ratio of the height to the length of the screen to display properly shader

    public List<BlackHole> BlackHoles = new List<BlackHole>();
    private List<BlackHole> _cleanUp = new List<BlackHole>();

    [System.Serializable]
    public class BlackHole
    {
        public GameObject Core; // The object whose position is taken as the position of the black hole.
        public float Radius; // The radius of the black hole measured in the same units as the other objects in the scene
        private Material _material;
        public bool ZeroSet = false;

        public Material Material
        {
            get
            {
                if (_material == null)
                {
#if UNITY_EDITOR
                    _material = new Material(Shader.Find("Gravitation Lensing Shader"));
#else
                    _material = new Material(Lens.StaticShader);
#endif
                    _material.hideFlags = HideFlags.HideAndDontSave;
                }

                return _material;
            }
        }

        public void Disable()
        {
            if (_material) DestroyImmediate(_material);
        }
    }

    public BlackHole AddBlackHole(GameObject core, float radius)
    {
        BlackHole blackHole = new BlackHole() {Core = core, Radius = radius};
        BlackHoles.Add(blackHole);
        return blackHole;
    }

    public BlackHole FindBlackHole(GameObject core)
    {
        foreach (BlackHole blackHole in BlackHoles)
        {
            if (blackHole.Core == core) return blackHole;
        }

        return null;
    }

    public void RemoveBlackHole(BlackHole blackHoleToRemove)
    {
        BlackHoles.Remove(blackHoleToRemove);
    }

    public void Awake()
    {
        if (Shader) StaticShader = Shader;
    }
    
	// Destroy temporary material when disabled.
	protected virtual void OnDisable () 
	{
	    foreach (BlackHole blackHole in BlackHoles)
	    {
	        blackHole.Disable();
	    }
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
	    if (Shader)
	    {
	        bool shouldBlit = false;
            RenderTexture tempSource;
	        RenderTexture tempDestination = source;


	        foreach (BlackHole blackHole in BlackHoles)
	        {
                if (!blackHole.Core)
	            {
	                // Game object has been destroyed, have this black hole removed from the list
                    _cleanUp.Add(blackHole);
                    continue;
	            }

	            if (blackHole.ZeroSet && blackHole.Radius == 0)
	            {
                    if (blackHole.Radius == 0)
                    {
                        // If the black hole's radius was and still is zero, skip this
                        continue;
                    }
                    else
                    {
                        // Otherwise, the radius is no longer zero and should be updated
                        blackHole.ZeroSet = false;
                    }
	            }
	            else if (blackHole.Radius == 0)
	            {
                    // If the black hole's radius is now zero, label it so that it is skipped
	                blackHole.ZeroSet = true;
                }

	            if (blackHole.Material)
	            {
	                shouldBlit = true;
	                tempSource = tempDestination;
                    tempDestination = RenderTexture.GetTemporary(destination.width, destination.height);

	                if (UseLocalPos)
	                {
	                    // Find the position of the black hole in screen coordinates
	                    Vector2 pos = new Vector2
	                    (
	                        this.GetComponent<Camera>().WorldToScreenPoint(blackHole.Core.transform.localPosition).x / this.GetComponent<Camera>().pixelWidth,
	                        this.GetComponent<Camera>().WorldToScreenPoint(blackHole.Core.transform.localPosition).y / this.GetComponent<Camera>().pixelHeight
	                    );

                        // Install all the required parameters for the shader
	                    blackHole.Material.SetVector("_Position", new Vector2(pos.x, pos.y));
	                    blackHole.Material.SetFloat("_Ratio", Ratio);
	                    blackHole.Material.SetFloat("_Rad", blackHole.Radius);
	                    blackHole.Material.SetFloat("_Distance", Vector3.Distance(blackHole.Core.transform.position, this.transform.position));

	                    // Apply to the resulting image.
	                    Graphics.Blit(tempSource, tempDestination, blackHole.Material);
	                }
	                else
	                {
	                    // Find the position of the black hole in screen coordinates
	                    Vector2 pos = new Vector2
	                    (
	                        this.GetComponent<Camera>().WorldToScreenPoint(blackHole.Core.transform.position).x / this.GetComponent<Camera>().pixelWidth,
	                        this.GetComponent<Camera>().WorldToScreenPoint(blackHole.Core.transform.position).y / this.GetComponent<Camera>().pixelHeight
	                    );

	                    // Install all the required parameters for the shader
	                    blackHole.Material.SetVector("_Position", new Vector2(pos.x, pos.y));
	                    blackHole.Material.SetFloat("_Ratio", Ratio);
	                    blackHole.Material.SetFloat("_Rad", blackHole.Radius);
	                    blackHole.Material.SetFloat("_Distance", Vector3.Distance(blackHole.Core.transform.position, this.transform.position));

	                    // Apply to the resulting image.
	                    Graphics.Blit(tempSource, tempDestination, blackHole.Material);
	                }
                }
	        }
            
            if (shouldBlit)
            {
                Graphics.Blit(tempDestination, destination);
                tempDestination.Release();
            }

	        if (_cleanUp.Count > 0)
	        {
	            foreach (BlackHole blackHole in _cleanUp)
	            {
	                RemoveBlackHole(blackHole);
	            }
	            _cleanUp.Clear();
	        }
        }
	}
}