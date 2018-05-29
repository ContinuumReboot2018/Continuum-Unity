using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection : MonoBehaviour 
{
	public MenuManager SelectAbilityMenu;
	public SaveAndLoadScript saveAndLoadScript;
	public RawImage AbilityImage;
	public Texture2D[] AbilityTextures;
	public GameObject[] ConfirmImages;

	void Start ()
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();

		saveAndLoadScript.LoadPlayerData ();
		SetAbilityId (saveAndLoadScript.SelectedAbility);
		ResetAllConfirmTextures (saveAndLoadScript.SelectedAbility);
		AbilityImage.texture = AbilityTextures [saveAndLoadScript.SelectedAbility];
	}

	public void PreviewAbilityId (int AbilityId)
	{
		saveAndLoadScript.SelectedAbility = AbilityId;
		AbilityImage.texture = AbilityTextures [AbilityId];
		Debug.Log ("Preview ability: " + AbilityId);
	}

	public void SetAbilityId (int AbilityId)
	{
		saveAndLoadScript.SelectedAbility = AbilityId;
		saveAndLoadScript.SavePlayerData ();
		ResetAllConfirmTextures (saveAndLoadScript.SelectedAbility);
		AbilityImage.texture = AbilityTextures [AbilityId];
		Debug.Log ("Ability ID set to: " + AbilityId);
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

	public void ForceReloadPlayerAbilityData ()
	{
		saveAndLoadScript.LoadPlayerData ();
		Debug.Log ("Loading Ability: " + saveAndLoadScript.SelectedAbility);
		AbilityImage.texture = AbilityTextures [saveAndLoadScript.SelectedAbility];
		saveAndLoadScript.SavePlayerData ();
	}

	public void OverrideButtonIndex ()
	{
		SelectAbilityMenu.SetButtonIndex (saveAndLoadScript.SelectedAbility);
	}
}