using UnityEngine;
using UnityEngine.UI;

public class TextureSwapper : MonoBehaviour 
{
	public Texture[] Textures;
	private RawImage rawImageComponent;

	void Start ()
	{
		rawImageComponent = GetComponent<RawImage> ();
	}

	public void UpdateTexture (int TextureId)
	{
		rawImageComponent.texture = Textures [TextureId];
	}
}