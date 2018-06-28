using UnityEngine;

public class StackZoneRow : MonoBehaviour 
{
	public StackZone[] Rows;
	public bool allRowsAreStacked;

	void Start ()
	{
		GetRows ();
		CheckRowStacked ();
		InvokeRepeating ("CheckRowStacked", 0, 0.5f);
	}

	void GetRows ()
	{
		Rows = GetComponentsInChildren<StackZone> ();
	}

	void CheckRowStacked ()
	{
		for (int i = 0; i < Rows.Length; i++)
		{
			// found a stack which is not occupied.
			if (Rows [i].isOccupied == false) 
			{
				allRowsAreStacked = false;
				return;
			}

			if (Rows [Rows.Length - 1].isOccupied == true) 
			{
				allRowsAreStacked = true;
			}
		}
	}
}