// Generates a block formation based on the color data from a texture.
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MiniBossFormationGenerator : MonoBehaviour 
{
	public Texture2D[] MiniBossMaps;
	[Tooltip ("Drop the texture in this slot in the inspector to read from.")]
	public Texture2D ChosenMap;
	[Header ("Color texture maps")]
	[Tooltip ("Set prefabs to spawn by color.")]
	public ColorToPrefab[] colorMappings;
	[Space (10)]
	[Tooltip ("The name of this instance of the boss.")]
	public string MiniBossName;

	[Header ("Transforms")]
	[Tooltip ("Set where the spawned prefab should parent to.")]
	public Transform ParentTransform;
	[Tooltip ("Spacing multiplier.")]
	public float Spacing = 1.5f;
	[Tooltip ("Scaling amount. (Default is 1.35).")]
	public Vector3 Scaling = new Vector3 (1.35f, 1.35f, 1.35f);
	[Header ("Centering")]
	[Tooltip ("Sets centre point on horizontal axis.")]
	public bool AutomaticallyCenterX;
	[Tooltip ("Sets centre point on vertical axis.")]
	public bool AutomaticallyCenterY;
	[Tooltip ("Offset to center the image. (Can be in decimals).")]
	public Vector2 Center;
	[Tooltip ("Brain Object in prefab.")]
	public MiniBoss MiniBossBrain;

	public List<GameObject> SpawnedBossParts;

	void Awake () 
	{
		ChooseMap ();
		AutoCenterImage (); // Checks if the image should auto center.
		GenerateMiniBossFormation (); // Does the creation of the formation.
	}

	void OnDestroy ()
	{
		if (Application.isPlaying == false) 
		{
			// Destroy each block only in edit mode.
			foreach (GameObject spawnedbosspart in SpawnedBossParts) 
			{
				DestroyImmediate (spawnedbosspart);
			}
		}

		SpawnedBossParts.Clear ();
	}

	void ChooseMap ()
	{
		int ChosenMapId = Random.Range (0, MiniBossMaps.Length);
		ChosenMap = MiniBossMaps[ChosenMapId];
	}

	void Start ()
	{
		ParentTransform.gameObject.SetActive (false);
		//Invoke ("TurnOnParentBossFormation", 3);
	}

	// Read the image then generate the formation.
	void GenerateMiniBossFormation ()
	{
		// Loop through all pixels on each row of pixels.
		for (int x = 0; x < ChosenMap.width; x++) 
		{
			// Loop through all pizels on each column.
			for (int y = 0; y < ChosenMap.height; y++) 
			{
				GenerateTile (x, y);
			}
		}
	}

	// Spawns a prefab by color to coordinate.
	void GenerateTile (int x, int y)
	{
		Color pixelColor = ChosenMap.GetPixel (x, y); // Reads the pixel data.

		if (pixelColor.a == 0) 
		{
			// The pixel is transparent. Let's ignore it.
			return;
		}

		// Found a matching color, spawn the relevant prefab to it at the correct position.
		foreach (ColorToPrefab colorMapping in colorMappings) 
		{
			// We found a matching color in the ChosenMap.
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
				SpawnedBossParts.Add (ColorMapObject);
			}
		}
	}

	// Allow optional centering.
	void AutoCenterImage ()
	{
		// Half of map height doesn't divide equally in two. There is a remainder, 
		// therefore the image height is odd number of pixels.
		if (ChosenMap.height % 2 != 0) 
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to nearest integer. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * ChosenMap.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? Mathf.Round (0.5f * ChosenMap.height - (0.333334f * Spacing)) : Center.y
			);
		} 

		else
		// The map height is even, divides equally in 2.
		{
			// Automatically center width: Half of map width minus a half.
			// Automatically center height: Half of map height minus a third of the spacing, then round to 2 decimal places. (It works).
			// Manually center: Use value from Vector2.
			Center = new Vector2 (
				AutomaticallyCenterX ? 0.5f * ChosenMap.width - 0.5f : Center.x, 
				AutomaticallyCenterY ? (float)System.Math.Round (0.5f * ChosenMap.height - (0.33333f * Spacing), 2) : Center.y
			);
		}
	}
}