using UnityEngine;

[System.Serializable]
public class ColorToPrefab
{
	[Tooltip ("Color to look up in the input texture.")]
	public Color color;
	[Tooltip ("Corresponding prefab to instantiate if the matching color is found.")]
	public GameObject prefab;
}