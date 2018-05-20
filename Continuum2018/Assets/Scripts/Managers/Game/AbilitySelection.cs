using UnityEngine;
using UnityEngine.UI;

public class AbilitySelection : MonoBehaviour 
{
	public SaveAndLoadScript saveAndLoadScript;

	public RawImage AbilityImage;
	public Texture2D[] AbilityTextures;

	void Start ()
	{
		// Find the saving script.
		saveAndLoadScript = GameObject.Find ("SaveAndLoad").GetComponent<SaveAndLoadScript> ();
		AbilityImage.texture = AbilityTextures [saveAndLoadScript.SelectedAbility];
	}

	public void SetAbilityId (int AbilityId)
	{
		saveAndLoadScript.SelectedAbility = AbilityId;
		saveAndLoadScript.SavePlayerData ();
		AbilityImage.texture = AbilityTextures [AbilityId];
		Debug.Log ("Ability ID set to: " + AbilityId);
	}
}