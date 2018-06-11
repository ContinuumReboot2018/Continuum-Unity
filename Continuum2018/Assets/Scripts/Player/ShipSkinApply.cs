using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipSkinApply : MonoBehaviour 
{
	public MenuManager SelectSkinMenu;
	public SaveAndLoadScript saveAndLoadScript;
	public MeshFilter playerMesh;
	public MeshRenderer playerRend;
	public Ship[] ships;
	public GameObject[] ConfirmImages;

	void Start ()
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();

		//saveAndLoadScript.LoadPlayerData ();
		ApplySkin (ships[saveAndLoadScript.SelectedSkin]);
		ResetAllConfirmTextures (saveAndLoadScript.SelectedSkin);
	}

	public void ApplySkin (Ship ship)
	{
		saveAndLoadScript.SelectedSkin = ship.ShipIndex;
		ResetAllConfirmTextures (ship.ShipIndex);
		playerMesh.mesh = ship.ShipMesh;
		playerRend.material = ship.ShipMaterial;
		Debug.Log ("Selected player skin from ID: " + ship.ShipIndex);

		// Don't bother saving unless its the menu scene.
		// Other scenes: just apply the skin.
		if (SceneManager.GetActiveScene ().name == "Menu") 
		{
			saveAndLoadScript.SavePlayerData ();
		}
	}

	public void ShipSkinPreview (Ship ship)
	{
		Debug.Log ("Previewing skin: " + ship.ShipIndex);
		playerMesh.mesh = ship.ShipMesh;
		playerRend.material = ship.ShipMaterial;
	}

	public void ResetAllConfirmTextures (int shipIndex)
	{
		for (int i = 0; i < ConfirmImages.Length; i++)
		{
			if (i != shipIndex) 
			{
				ConfirmImages [i].SetActive (false);
			}

			else 
			
			{
				ConfirmImages [i].SetActive (true);
			}
		}
	}

	public void ForceReloadPlayerShipData ()
	{
		saveAndLoadScript.LoadPlayerData ();
		Debug.Log ("Loading Skin: " + saveAndLoadScript.SelectedSkin);
		playerMesh.mesh = ships[saveAndLoadScript.SelectedSkin].ShipMesh;
		playerRend.material = ships[saveAndLoadScript.SelectedSkin].ShipMaterial;
		saveAndLoadScript.SavePlayerData ();
	}

	public void OverrideButtonIndex ()
	{
		SelectSkinMenu.SetButtonIndex (saveAndLoadScript.SelectedSkin);
	}
}