// Generates a block formation based on the color data from a texture.
using UnityEngine;

public class MiniBossFormationGenerator : MonoBehaviour 
{
	public Texture2D map; // Drop the texture in this slot in the inspector to read from.
	public ColorToPrefab[] colorMappings; // Set prefabs to spawn by color.
	[Space (10)]
	public Transform ParentTransform; // Set where the spawned prefab should parent to.
	public float Spacing = 1.5f; // Spacing multiplier.
	public Vector3 Scaling = new Vector3 (1.35f, 1.35f, 1.35f); // Scaling amount. (Default is 1.35).
	public bool AutomaticallyCenter; // Sets centre point to half width and half height if enabed.
	public Vector2 Center; // Offset to center the image. (Can be in decimals).


	void Start () 
	{
		GenerateMiniBossFormation ();

		if (AutomaticallyCenter == true) 
		{
			AutoCenterImage ();
		}
	}

	void GenerateMiniBossFormation ()
	{
		// Loop through all pixels on each row of pixels.
		for (int x = 0; x < map.width; x++) 
		{
			// Loop through all pizels on each column.
			for (int y = 0; y < map.height; y++) 
			{
				GenerateTile (x, y);
			}
		}
	}

	void GenerateTile (int x, int y)
	{
		Color pixelColor = map.GetPixel (x, y);

		if (pixelColor.a == 0) 
		{
			// The pixel is transparent. Let's ignore it.
			return;
		}

		foreach (ColorToPrefab colorMapping in colorMappings) 
		{
			// We found a matching color in the map.
			if (colorMapping.color.Equals (pixelColor)) 
			{
				// Converts pixel position to unity transform position units.
				Vector2 position = new Vector2 ((x-Center.x) * Spacing, (y-Center.y) * Spacing);

				// Creates the relevant prefab at the set position.
				GameObject ColorMapObject = Instantiate (colorMapping.prefab, position, Quaternion.identity, ParentTransform);
				ColorMapObject.transform.localScale = new Vector3 (Scaling.x, Scaling.y, Scaling.z);
			}
		}
	}

	void AutoCenterImage ()
	{
		Center = new Vector2 (0.5f * map.width, 0.5f * map.height);
	}
}
