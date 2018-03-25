// Generates a block formation based on the color data from a texture.
using UnityEngine;

[ExecuteInEditMode]
public class MiniBossFormationGenerator : MonoBehaviour 
{
	public Texture2D map; // Drop the texture in this slot in the inspector to read from.
	[Header ("Color texture  maps")]
	public ColorToPrefab[] colorMappings; // Set prefabs to spawn by color.
	[Space (10)]
	[Header ("Transforms")]
	public Transform ParentTransform; // Set where the spawned prefab should parent to.
	public float Spacing = 1.5f; // Spacing multiplier.
	public Vector3 Scaling = new Vector3 (1.35f, 1.35f, 1.35f); // Scaling amount. (Default is 1.35).
	[Header ("Centering")]
	public bool AutomaticallyCenterX; // Sets centre point on horizontal axis.
	public bool AutomaticallyCenterY; // Sets centre point on vertical axis.
	public Vector2 Center; // Offset to center the image. (Can be in decimals).
	public MiniBoss MiniBossBrain;

	void Awake () 
	{
		AutoCenterImage (); // Checks if the image should auto center.
		GenerateMiniBossFormation (); // Does the creation of the formation.
	}

	// Read the image then generate the formation.
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

	// Spawns a prefab by color to coordinate.
	void GenerateTile (int x, int y)
	{
		Color pixelColor = map.GetPixel (x, y); // Reads the pixel data.

		if (pixelColor.a == 0) 
		{
			// The pixel is transparent. Let's ignore it.
			return;
		}

		// Found a matching color, spawn the relevant prefab to it at the correct position.
		foreach (ColorToPrefab colorMapping in colorMappings) 
		{
			// We found a matching color in the map.
			if (colorMapping.color.Equals (pixelColor)) 
			{
				// Converts pixel position to unity transform position units.
				Vector2 position = new Vector2 (
					(x * Spacing) - (Center.x * Spacing), 
					(y * Spacing) - (Center.y * Spacing)
				);

				// Creates the relevant prefab at the set position.
				GameObject ColorMapObject = Instantiate (colorMapping.prefab, position, Quaternion.identity, ParentTransform);
				ColorMapObject.transform.localPosition = new Vector3 (position.x, position.y, 0);
				ColorMapObject.transform.localScale = new Vector3 (Scaling.x, Scaling.y, Scaling.z);
				ColorMapObject.GetComponent<Block> ().miniBoss = MiniBossBrain;
			}
		}
	}

	// Allow optional centering.
	void AutoCenterImage ()
	{
		// Half of map height doesn't divide equally in two. There is a remainder, 
		// therefore the image height is odd number of pixels.
		if (map.height % 2 != 0) 
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to nearest integer. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * map.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? Mathf.Round (0.5f * map.height - (0.333334f * Spacing)) : Center.y
			);
		} 

		else
		// The map height is even, divides equally in 2.
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to 2 decimal places. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * map.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? (float)System.Math.Round (0.5f * map.height - (0.33333f * Spacing), 2) : Center.y
			);
		}
	}
}