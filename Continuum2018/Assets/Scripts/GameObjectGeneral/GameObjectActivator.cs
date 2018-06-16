using UnityEngine;
using UnityEngine.Events;

public class GameObjectActivator : MonoBehaviour
{
	public UnityEvent OnActivate;
	public UnityEvent OnDeactivate;

	public void ActivateObjects ()
	{
		OnActivate.Invoke ();
	}

	public void DeactivateObjects ()
	{
		OnDeactivate.Invoke ();
	}
}