using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection : MonoBehaviour 
{
	public MenuManager SelectAbilityMenu;
	public RawImage AbilityImage;
	public Texture2D[] AbilityTextures;
	public GameObject[] ConfirmImages;

	void Start ()
	{
		//SaveAndLoadScript.Instance.LoadPlayerData ();
		//SetAbilityId (SaveAndLoadScript.Instance.SelectedAbility);
		SetAbilityIdNoSave (SaveAndLoadScript.Instance.SelectedAbility);
		ResetAllConfirmTextures (SaveAndLoadScript.Instance.SelectedAbility);
		AbilityImage.texture = AbilityTextures [SaveAndLoadScript.Instance.SelectedAbility];
	}

	public void PreviewAbilityId (int AbilityId)
	{
		SaveAndLoadScript.Instance.SelectedAbility = AbilityId;
		AbilityImage.texture = AbilityTextures [AbilityId];
		Debug.Log ("Preview ability: " + AbilityId);
	}

	void SetAbilityIdNoSave (int AbilityId)
	{
		SaveAndLoadScript.Instance.SelectedAbility = AbilityId;
		ResetAllConfirmTextures (SaveAndLoadScript.Instance.SelectedAbility);
		AbilityImage.texture = AbilityTextures [AbilityId];
		Debug.Log ("Ability ID set to: " + AbilityId);
	}

	public void SetAbilityId (int AbilityId)
	{
		SaveAndLoadScript.Instance.SelectedAbility = AbilityId;
		SaveAndLoadScript.Instance.SavePlayerData ();
		ResetAllConfirmTextures (SaveAndLoadScript.Instance.SelectedAbility);
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
		SaveAndLoadScript.Instance.LoadPlayerData ();
		Debug.Log ("Loading Ability: " + SaveAndLoadScript.Instance.SelectedAbility);
		AbilityImage.texture = AbilityTextures [SaveAndLoadScript.Instance.SelectedAbility];
		SaveAndLoadScript.Instance.SavePlayerData ();
	}

	public void OverrideButtonIndex ()
	{
		SelectAbilityMenu.SetButtonIndex (SaveAndLoadScript.Instance.SelectedAbility);
	}
}