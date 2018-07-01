using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipSkinApply : MonoBehaviour 
{
	public MenuManager SelectSkinMenu;
	public MeshFilter playerMesh;
	public MeshRenderer playerRend;
	public Ship[] ships;
	public GameObject[] ConfirmImages;

	void Start ()
	{
		ApplySkinStartNoSave (ships [SaveAndLoadScript.Instance.SelectedSkin]);
		ResetAllConfirmTextures (SaveAndLoadScript.Instance.SelectedSkin);
	}

	void ApplySkinStartNoSave (Ship ship)
	{
		SaveAndLoadScript.Instance.SelectedSkin = ship.ShipIndex;
		ResetAllConfirmTextures (ship.ShipIndex);
		playerMesh.mesh = ship.ShipMesh;
		playerRend.material = ship.ShipMaterial;
		Debug.Log ("Selected player skin from ID: " + ship.ShipIndex);
	}

	public void ApplySkin (Ship ship)
	{
		SaveAndLoadScript.Instance.SelectedSkin = ship.ShipIndex;
		ResetAllConfirmTextures (ship.ShipIndex);
		playerMesh.mesh = ship.ShipMesh;
		playerRend.material = ship.ShipMaterial;
		Debug.Log ("Selected player skin from ID: " + ship.ShipIndex);

		// Don't bother saving unless its the menu scene.
		// Other scenes: just apply the skin.
		if (SceneManager.GetActiveScene ().name == "Menu") 
		{
			SaveAndLoadScript.Instance.SavePlayerData ();
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
		SaveAndLoadScript.Instance.LoadPlayerData ();
		Debug.Log ("Loading Skin: " + SaveAndLoadScript.Instance.SelectedSkin);
		playerMesh.mesh = ships[SaveAndLoadScript.Instance.SelectedSkin].ShipMesh;
		playerRend.material = ships[SaveAndLoadScript.Instance.SelectedSkin].ShipMaterial;
		SaveAndLoadScript.Instance.SavePlayerData ();
	}

	public void OverrideButtonIndex ()
	{
		SelectSkinMenu.SetButtonIndex (SaveAndLoadScript.Instance.SelectedSkin);
	}
}