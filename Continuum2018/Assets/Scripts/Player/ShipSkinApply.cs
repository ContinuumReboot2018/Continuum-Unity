using UnityEngine;

public class ShipSkinApply : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;
	public MeshFilter playerMesh;
	public MeshRenderer playerRend;

	public Ship[] ships;

	void Start ()
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();

		ApplySkin (ships[saveAndLoadScript.SelectedSkin]);
	}

	public void ApplySkin (Ship ship)
	{
		Debug.Log ("Loading Skin: " + saveAndLoadScript.SelectedSkin);
		saveAndLoadScript.SelectedSkin = ship.ShipIndex;
		playerMesh.mesh = ship.ShipMesh;
		playerRend.material = ship.ShipMaterial;
		saveAndLoadScript.SavePlayerData ();
	}
}