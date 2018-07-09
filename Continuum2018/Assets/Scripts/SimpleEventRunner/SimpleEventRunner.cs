using UnityEngine;
using System.Collections.Generic;

public class SimpleEventRunner : MonoBehaviour 
{
	public List<UnityEventDrawer> UnityEvents;

	public void InvokeEventId (int EventId)
	{
		UnityEvents [EventId].unityEvent.Invoke ();
	}

	public void InvokeEventString (string EventName)
	{
		for (int i = 0; i < UnityEvents.Count; i++) 
		{
			if (UnityEvents [i].name == EventName) 
			{
				UnityEvents [i].unityEvent.Invoke ();
				break;
			}
		}
	}
}
